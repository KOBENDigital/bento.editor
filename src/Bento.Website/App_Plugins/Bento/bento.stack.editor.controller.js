(function () {
	'use strict';

	function bentoStackEditorController($scope, $sce, editorService, notificationsService, contentResource, assetsService, localizationService, overlayService) {



		var vm = this;
		vm.culture = $scope.model.culture;
		vm.layouts = [];
		vm.sorting = false;
		vm.firstSort = true;
		vm.itemUpdating = true;
		vm.remove = remove;
		vm.setSort = setSort;
		vm.addLayout = addLayout;
		vm.openSettings = openSettings;
		vm.openLayouts = openLayouts;
		vm.toggleDeleteConfirm = toggleDeleteConfirm;
		vm.setLayout = setLayout;
		vm.getAvailableLayouts = getAvailableLayouts;

		if ($scope.model.config.usePreviewJs && $scope.model.config.jsFilePath && $scope.model.config.jsFilePath !== null && $scope.model.config.jsFilePath !== '') {
			assetsService.loadJs($scope.model.config.jsFilePath, $scope).then(function () {

			});
		}

		if ($scope.model.config.useCssFile && $scope.model.config.fontCssUrls && $scope.model.config.fontCssUrls !== null && $scope.model.config.fontCssUrls !== '') {
			vm.cssFonts = $scope.model.config.fontCssUrls.split('*');
		}

		vm.trustSrc = function (src) {
			return $sce.trustAsResourceUrl(src);
		};

		$scope.model.hideLabel = $scope.model.config.hideLabel;

		if ($scope.model.value || !_.isEmpty($scope.model.value)) {

			var layouts = getAvailableLayouts();

			vm.layouts = _.map($scope.model.value,
				function (item) {
					var layout = _.find(layouts, function (lo) {
						return lo.alias === item.alias;
					});

					if (!layout) {
						return item;
					}

					item.layout = layout;
					item.name = layout.name;
					item.allowSort = Boolean(Number(layout.allowSort));

					//setup the allowed types again.
					angular.forEach(item.areas, function (val, key) {
						var area = layout.areas[key];

						val.allowedElementTypes = area.allowedElementTypes !== undefined ? area.allowedElementTypes : '';
						val.allowedContentTypes = area.allowedContentTypes !== undefined ? area.allowedContentTypes : '';
					});

					//this could come from layout settings if not set use the generic icon
					item.icon = layout.icon || "icon-layout";

					return item;
				});
		}

		function createEmptyArea(area) {
			var emptyArea = {};
			emptyArea.name = "...";
			emptyArea.icon = "icon-brick";
			emptyArea.deleteConfirmVisible = false;
			emptyArea.id = undefined;
			emptyArea.key = undefined;
			emptyArea.contentData = undefined;
			emptyArea.alias = area.alias;
			emptyArea.allowedContentTypes = area.allowedContentTypes;
			emptyArea.allowedElementTypes = area.allowedElementTypes;
			return emptyArea;
		}

		function addLayout(index) {
			var item = {
				icon: "icon-layout",
				deleteConfirmVisible: false
			};

			openLayouts(item, index, false);
		}

		function toggleDeleteConfirm(index) {



			localizationService.localizeMany(["content_nestedContentDeleteItem", "general_delete", "general_cancel", "contentTypeEditor_yesDelete"]).then(function (data) {
				const overlay = {
					title: data[1],
					content: data[0],
					closeButtonLabel: data[2],
					submitButtonLabel: data[3],
					submitButtonStyle: "danger",
					close: () => overlayService.close(),
					submit: () => {
						remove(index);
						overlayService.close();
					}
				};

				overlayService.open(overlay);
			});

		}

		function remove(index) {
			vm.layouts.splice(index, 1);
			updateSavedValue();
		}

		function getAvailableLayouts() {
			var availableLayouts;
			if (typeof $scope.model.config.layouts !== "undefined" &&
				$scope.model.config.layouts !== null &&
				$scope.model.config.layouts.length >= 2) {
				availableLayouts = JSON.parse($scope.model.config.layouts);
			} else {
				availableLayouts = [];
			}

			return availableLayouts;
		}

		function getDefaultSettings() {
			// we are just setting this to an empty object
			// we could go and pick this up from content in the tree somewhere
			var defaultSettings = {};
			return defaultSettings;
		}

		function openLayouts(item, index) {

			var availableLayouts = getAvailableLayouts();

			if (availableLayouts.length === 1) {
				//dont bother opening we only have 1;
				vm.setLayout(item, availableLayouts[0], index);
				return;
			}

			var options = {
				view: "/App_Plugins/Bento/bento.layouts.editor.html",
				availableLayouts: availableLayouts,
				blockLayout: item, //todo this should be set as saved layout if present
				size: "small",
				submit: (model) =>  {
					if (model === undefined || model === null) {
						notificationsService.error("No Layout has been selected for '" + item.name + "'",
							"Please make sure you have selected one of the available layouts");
						return;
					}

					vm.setLayout(item, model, index);

					editorService.close();
					notificationsService.removeAll(); // just to reduce the noise

					notificationsService.info("Layout updated for layout '" + item.name + "'",
						"Don't forget to publish to save your updates!");

				},
				close: () => {
					editorService.close();
				}
			};

			editorService.open(options);

		}

		function setLayout(item, layout, index) {
			//used to represent the layout in the stack
			item.name = layout.name;
			item.alias = layout.alias;

			//sets the data to layout the layout
			item.layout = layout;

			//sets up the layouts data if there is no areas defined
			if (!item.areas) {
				item.areas = [];
				for (let i = 0; i < layout.areas.length; i++) {
					item.areas.push(createEmptyArea(layout.areas[i]));
				}
				item.settings = getDefaultSettings();
			}
			// if there are more areas in the layout that items in the array
			else if (item.areas && item.areas.length < layout.areas.length) {
				for (let i = item.areas.length; i < layout.areas.length; i++) {
					item.areas.push(createEmptyArea(layout.areas[i]));
				}
			}
			// if there are more areas in the array than there are in the layout
			// this is destructive and the user should be warned if they chose a layout that is smaller than the data
			else if (item.areas && item.areas.length > layout.areas.length) {
				let reduction = layout.areas.length - item.areas.length;
				item.areas.splice(reduction);
			}

			if (index !== undefined) {
				vm.layouts.splice(index, 0, item);
			}
			updateSavedValue();
		}

		function buildSettingsData(node) {
			var value = {
				contentTypeAlias: node.contentTypeAlias,
				key: node.key
			};

			for (var v = 0; v < node.variants.length; v++) {
				var variant = node.variants[v];
				for (var t = 0; t < variant.tabs.length; t++) {
					var tab = variant.tabs[t];
					for (var p = 0; p < tab.properties.length; p++) {
						var prop = tab.properties[p];
						if (typeof prop.value !== "function") {
							value[prop.alias] = prop.value;
						}
					}
				}
			}
			return value;
		}


		function openSettings(layout) {
			var layoutSettings = {};
			if (typeof layout.settings !== "undefined" &&
				layout.settings !== null) {
				layoutSettings = layout.settings;
			} else {
				layoutSettings = {};
			}

			var options = {
				title: 'Edit',
				embed: true,
				size: 'small',
				nodeData: layoutSettings,
				documentTypeAlias: layout.layout.layoutSettings,//$scope.model.config.layoutSettingsDoctypeAlias,
				documentTypeName: 'Layout Settings',
				view: '/App_Plugins/Bento/bento.edit.html',
				submit: (model) => {

					var value = buildSettingsData(model.node);
					layout.settings = value; // this is what we will save

					$scope.$broadcast("bentoStackSyncVal");
					editorService.close();

				},
				close: () => {
					editorService.close();
				}
			};
			editorService.open(options);
		}

		function setSort() {
			vm.sorting = !vm.sorting;
		}

		vm.sortOptions = {
			handle: '> .bento-stack-item .bento-stack-item-handle',
			stop: function (e, ui) {
				updateSavedValue();
			},
			'ui-floating': true,
			start: function (e, ui) {
				if (vm.firstSort) {  // Call a refresh on ui-sortable on drag of first element.
					$(ui.helper).parent().sortable("refreshPositions");
					vm.firstSort = false;
				}
			}
		};

		function updateSavedValue() {
			//clear the value
			$scope.model.value = [];

			angular.forEach(vm.layouts,
				function (val, key) {
					var layout = {
						alias: val.alias,
						settings: val.settings
					};

					layout.areas = _.map(val.areas,
						function (area) {
							return {
								id: area.id,
								key: area.key,
								name: area.name,
								alias: area.alias,
								contentData: area.contentData
							};
						}
					);

					$scope.model.value.push(layout);
				});
			$scope.propertyForm.$setDirty();
		}

		var unsubscribe = $scope.$on("bentoSyncStack", function (ev, args) {
			vm.layouts[args.index].areas = args.areas;
			updateSavedValue();

		});

		$scope.$on('$destroy', function () {
			unsubscribe();
		});
	}

	angular.module('umbraco').controller('bento.stack.editor.controller', bentoStackEditorController);
})();
