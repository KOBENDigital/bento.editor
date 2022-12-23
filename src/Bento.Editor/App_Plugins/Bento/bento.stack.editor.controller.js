(function () {
	'use strict';

	angular.module('umbraco').run(['clipboardService', function (clipboardService) {

		function resolveBentoItemForPaste(prop, propClearingMethod) {

			// if we got an array, and it has a entry with ncContentTypeAlias this means that we are dealing with a NestedContent property data.
			if ((Array.isArray(prop) && prop.length > 0 && prop[0].ncContentTypeAlias !== undefined)) {

				for (var i = 0; i < prop.length; i++) {
					var obj = prop[i];

					// generate a new key.
					obj.key = String.CreateGuid();

					// Loop through all inner properties:
					for (var k in obj) {
						propClearingMethod(obj[k], clipboardService.TYPES.RAW);
					}
				}
			}
		}

		clipboardService.registerPastePropertyResolver(resolveBentoItemForPaste, clipboardService.TYPES.RAW);
	}]);


	function bentoStackEditorController($scope, $sce, editorService, notificationsService, assetsService, localizationService, overlayService, clipboardService) {

		clipboardService.registrerTypeResolvers();

		var vm = this;
		vm.culture = $scope.model.culture;
		vm.layouts = [];
		vm.sorting = false;
		vm.firstSort = true;
		vm.itemUpdating = true;
		vm.remove = remove;
		vm.setSort = setSort;
		vm.sortUp = sortUp;
		vm.sortDown = sortDown;
		vm.addLayout = addLayout;
		vm.openSettings = openSettings;
		vm.openLayouts = openLayouts;
		vm.toggleDeleteConfirm = toggleDeleteConfirm;
		vm.setLayout = setLayout;
		vm.getAvailableLayouts = getAvailableLayouts;
		vm.copyLayout = copyLayout;
		vm.dataTypeKey = $scope.umbProperty.property.dataTypeKey;
		vm.maxEditorWidth = $scope.model.config.maxEditorWidth ?? "100%";

		if ($scope.model.config.usePreviewJs && $scope.model.config.jsFilePath && $scope.model.config.jsFilePath !== null && $scope.model.config.jsFilePath !== '') {
			//todo: we're replacing the 'wwwroot' that gets appended in v9/core... not ideal, we're looking into getting the tree picker to start in the wwwroot https://our.umbraco.com/forum/using-umbraco-and-getting-started//108099-is-it-possible-to-start-the-editor-service-file-picker-in-the-wwwroot
			//console.log('loading site js');
			assetsService.loadJs($scope.model.config.jsFilePath.replace('/wwwroot', ''), $scope).then(function () { });
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


		function guid() {
			function s4() {
				return Math.floor((1 + Math.random()) * 0x10000)
					.toString(16)
					.substring(1);
			}
			return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
				s4() + '-' + s4() + s4() + s4();
		}

		function copyLayout(index) {

			//option the dialogue to get a friendly name
			var options = {
				view: "/App_Plugins/Bento/bento.copy.editor.html",
				size: "small",
				submit: (model) => {

					var copy = angular.copy(vm.layouts[index]);
					copy.allowInProperties = vm.dataTypeKey;
					clipboardService.copy(clipboardService.TYPES.RAW, 'layout', copy, `${model}`, copy.icon, guid());
					editorService.close();

				},
				close: () => {
					editorService.close();
				}
			};

			editorService.open(options);
		}

		function getAvailableInClipboard() {

			return clipboardService.retriveEntriesOfType(clipboardService.TYPES.RAW, ['layout'])
				.filter(
					(item) => {

						if (item.data.allowInProperties == vm.dataTypeKey) {
							return true;
						}
						return false;
					}
				);
		}


		function addLayout(index) {
			var item = {
				icon: "icon-layout",
				deleteConfirmVisible: false
			};
			openLayouts(item, false, index);
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

		function openLayouts(item, hideClipboard, index) {

			var availableLayouts = getAvailableLayouts();
			var availableInClipboard = !hideClipboard ? getAvailableInClipboard() : [];

			if (availableLayouts.length === 1 && availableInClipboard.length === 0) {
				//dont bother opening we only have 1;
				vm.setLayout(item, availableLayouts[0], index);
				return;
			}

			var options = {
				view: "/App_Plugins/Bento/bento.layouts.editor.html",
				availableLayouts: availableLayouts,
				availableInClipboard: availableInClipboard,
				blockLayout: item, //todo this should be set as saved layout if present
				size: "small",
				submit: (model) => {
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

		function setLayout(item, model, index) {

			// this came from a paste
			if (model.layout) {

				item.name = model.name;
				item.alias = model.alias;
				item.layout = model.layout;
				item.settings = model.settings;
				item.areas = model.areas;

			} else {

				let layout = model;

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
			handle: '> .bento-stack-item .bento-stack-item-handle .bento-stack-item-title',
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

		function sortUp(index) {
			var layout = vm.layouts.splice(index, 1)[0];
			vm.layouts.splice(index - 1, 0, layout);
			updateSavedValue()
		}

		function sortDown(index) {
			var layout = vm.layouts.splice(index, 1)[0];
			vm.layouts.splice(index + 1, 0, layout);
			updateSavedValue()
		}

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
