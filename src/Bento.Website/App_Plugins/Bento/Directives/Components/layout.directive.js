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
		clipboardService.registerPastePropertyResolver(resolveBentoItemForPaste, clipboardService.TYPES.RAW)
	}]);

	function bentoLayoutDirective($sce, $http, localizationService, overlayService, clipboardService, editorService) {
		var directive = {
			restrict: 'E',
			templateUrl: '/App_Plugins/Bento/Views/Components/bento-layout.html',
			scope: {
				name: '=',
				alias: '=',
				layout: '=',
				icon: '=',
				areas: '=',
				allowsort: '=',
				config: '=',
				index: '=',
				settings: '=',
				culture: '=',
				index: '=?'
			},

			controller: function ($scope, $element) {

				clipboardService.registrerTypeResolvers();

				$scope.initLayout = initLayout;
				$scope.toggleDeleteConfirm = toggleDeleteConfirm;
				$scope.remove = remove;
				$scope.updateLayoutStyle = updateLayoutStyle;
				$scope.sorting = false;
				$scope.setSort = setSort;
				$scope.firstSort = true;
				$scope.copyArea = copyArea;
				$scope.pasteArea = pasteArea;

				$scope.allowSort = Boolean(Number($scope.layout.allowSort));

				if ($scope.allowSort) {
					$scope.sortOptions = {
						handle: '.bento-layout-item-title',
						stop: function (e, ui) {
							ui.item.parent().find('.drop').removeClass('drop');

							angular.forEach($scope.areas, function (area, $index) {
								area.alias = $scope.layout.areas[$index].alias;
							});

							$scope.setSort(false);
							updateStack();
						},
						change: function (e, ui) {

							$(ui.helper).parent().find('.drop').removeClass('drop');
							var helper = $(ui.helper).parent().find('.ui-sortable-helper');
							var placeholder = $(ui.helper).parent().find('.ui-sortable-placeholder');
							var lastIndex = $(ui.helper).parent().children().length - 1;

							if (placeholder.index() === 0 || placeholder.index() === 1 && placeholder.next().index() === lastIndex) {

								if (helper.index() < placeholder.index()) {
									placeholder.prev().addClass('drop');
								} else {
									placeholder.next().addClass('drop');
								}
							}
							else if (placeholder.index() === 1 && helper.index() === 0 || placeholder.next().index() === -1 || placeholder.next().index() === lastIndex && helper.index() !== lastIndex) {
								placeholder.prev().addClass('drop');
							} else {
								placeholder.next().addClass('drop');
							}
						},
						update: function (e, ui) {
							ui.item.parent().find('.drop').removeClass('drop');
						},
						'ui-floating': true,
						start: function (e, ui) {
							$(ui.helper).parent().find('.drop').removeClass('drop');
							if ($scope.firstSort) {  // Call a refresh on ui-sortable on drag of first element.
								$(ui.helper).parent().sortable("refreshPositions");
								$scope.firstSort = false;
							}
						}
					};
				} else {
					$scope.sortOptions = {
						disabled: true
					};
				}

				function setSort(sort) {
					$scope.sorting = sort;
				}

				function updateLayoutStyle() {
					if ($scope.settings.contentTypeAlias !== undefined && $scope.config.useCssFile && $scope.config.useBlockSettingsCss && $scope.config.cssFilePath) {
						let url = '/umbraco/backoffice/Bento/LoadEmbeddedStyler';

						let data = {
							guid: guid(),
							contentTypeAlias: $scope.settings.contentTypeAlias,
							dataJson: JSON.stringify($scope.settings),
							culture: typeof ($scope.culture) !== 'undefined' ? $scope.culture : null
						};

						$http.post(url, data).then(function (response) {
							$scope.getStyler = function () {
								return $sce.trustAsHtml(response.data);
							};
						}).catch(function (error) {
							console.log(error);
						}).finally(function () {
						});
					}
				}

				function initLayout() {
					$scope.layoutStyle = '';
					//setup the columns configuration
					$scope.layoutStyle += 'grid-template-columns: ';
					$scope.layoutStyle += $scope.layout.columns.join(' ') + ";";
					$scope.areaStyles = [];

					for (var i = 0; i < $scope.layout.areas.length; i++) {
						var area = $scope.layout.areas[i];

						var style = "grid-column: " + area.column + ";";
						style += " ";
						style += "grid-row: " + area.row + ";";

						$scope.areaStyles.push(style);
					}
				}

				function copyArea(index) {
					var options = {
						view: "/App_Plugins/Bento/bento.copy.editor.html",
						size: "small",
						submit: (model) => {
							var copy = angular.copy($scope.areas[index]);
							clipboardService.copy(clipboardService.TYPES.RAW, 'layoutArea', copy, model, copy.icon, copy.key);
							editorService.close();
						},
						close: () => {
							editorService.close();
						}
					};

					editorService.open(options);
				}

				function pasteAreaFromClipboard(item) {

					//update alias and give item new key for tracking
					var targetAreaAlias = $scope.areas[item.index].alias;
					item.data.alias = targetAreaAlias;
					item.data.key = guid();

					//write the item to the stack
					$scope.areas[item.index] = item.data;

					updateStack();
				}

				function pasteArea(index) {
					const dialog = {
						orderBy: "$index",
						view: "itempicker",

						size: "small",
						clickPasteItem: function (item) {
							pasteAreaFromClipboard(item);
							overlayService.close();
						},
						close: function () {
							overlayService.close();
						}
					};

					dialog.pasteItems = [];
					dialog.availableItems = [];

					var allowedItems = $scope.layout.areas[index].allowedElementTypes.split(',').concat($scope.layout.areas[index].allowedContentTypes.split(','));
					var entriesForPaste = clipboardService.retriveEntriesOfType(clipboardService.TYPES.RAW, ['layoutArea'])
						.filter(
							(item) => {
								if (item.data.contentData) {
									return allowedItems.indexOf(item.data.contentData.contentTypeAlias) > -1;
								} else {
									return allowedItems.indexOf(item.data.contentNode.metaData.ContentTypeAlias) > -1;
								}
								return false;
							}
						);

					_.each(entriesForPaste, function (entry) {

						dialog.pasteItems.push({
							date: entry.date,
							name: entry.label,
							data: entry.data,
							icon: entry.icon,
							index: index
						});
					});

					dialog.pasteItems.sort((a, b) => {
						return b.date - a.date;
					});

					dialog.hideHeader = true;

					dialog.clickClearPaste = function ($event) {
						$event.stopPropagation();
						$event.preventDefault();
						clipboardService.clearEntriesOfType(clipboardService.TYPES.RAW, ['layoutArea']);
						dialog.pasteItems = [];// This dialog is not connected via the clipboardService events, so we need to update manually.
						dialog.hideHeader = false;
					};

					overlayService.open(dialog);
				}

				function toggleDeleteConfirm(index) {
					localizationService.localizeMany(["content_nestedContentDeleteItem", "general_delete", "general_cancel", "contentTypeEditor_yesDelete"]).then(function (data) {
						const overlay = {
							title: data[1],
							content: data[0],
							closeButtonLabel: data[2],
							submitButtonLabel: data[3],
							submitButtonStyle: "danger",
							close: () => {
								overlayService.close();
							},
							submit: () => {
								remove(index);
								overlayService.close();
							}
						};

						overlayService.open(overlay);
					});
				}

				function remove(index) {
					//set the item back to blank
					$scope.areas[index].id = undefined;
					$scope.areas[index].key = undefined;
					$scope.areas[index].contentNode = undefined;
					$scope.areas[index].contentData = undefined;

					//needs translation
					$scope.areas[index].name = "...";
					$scope.areas[index].icon = "icon-brick";
					$scope.areas[index].deleteConfirmVisible = false;

					$scope.$broadcast("bentoClearVal", index);

					updateStack();
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

				initLayout();
				updateLayoutStyle();

				var updateStack = function () {
					var updateObj = {
						areas: $scope.areas,
						index: $scope.index
					}

					$scope.$emit("bentoSyncStack", updateObj);
				}

				var unsubscribe = $scope.$on("bentoSyncVal", function (ev, args) {

					$scope.areas[args.index].id = args.id;
					$scope.areas[args.index].key = args.key;

					if (args.id == 0) {
						$scope.areas[args.index].contentNode = args.contentNode ? args.contentNode : undefined;
						$scope.areas[args.index].contentData = args.contentData;

						//needs translation
						$scope.areas[args.index].name = args.contentData.name;
						$scope.areas[args.index].icon = args.contentData.icon;
					}

					$scope.areas[args.index].deleteConfirmVisible = true;

					updateStack();
				});

				$scope.$on('$destroy', function () {
					unsubscribe();
				});

				$scope.$watch('layout', function (newValue, oldValue) {
					$scope.initLayout();
				}, true);

				$scope.$watch('settings', function (newValue, oldValue) {
					$scope.updateLayoutStyle();
				}, true);
			}
		};

		return directive;
	}

	angular.module('umbraco.directives').directive('bentoLayout', bentoLayoutDirective);
})();