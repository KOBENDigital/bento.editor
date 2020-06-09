(function () {
	'use strict';

	function bentoLayoutDirective(notificationsService, $routeParams, $sce, $http) {
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
				updating: '=',
				index: '=',
				settings: '='
			},

			controller: function ($scope, $element) {

				$scope.initLayout = initLayout;
				$scope.toggleDeleteConfirm = toggleDeleteConfirm;
				$scope.remove = remove;
				$scope.updateLayoutStyle = updateLayoutStyle;
				$scope.sorting = false;
				$scope.setSort = setSort;
				$scope.firstSort = true;

				$scope.allowSort = Boolean(Number($scope.layout.allowSort));

				if ($scope.allowSort) {
					$scope.sortOptions = {
						handle: '.bento-layout-item-title',
						stop: function (e, ui) {
							$scope.itemUpdating = true;

							ui.item.parent().find('.drop').removeClass('drop');
							//angular.forEach($scope.layout.areas, function (val, key) {
							//	$scope.areas[key].alias = val.alias;
							//	$scope.areas[key].allowedElementTypes = val.allowedElementTypes;
							//	$scope.areas[key].allowedContentTypes = val.allowedContentTypes;
							//});

							$scope.setSort(false);
						},
						change: function (e, ui) {
							$(ui.helper).parent().find('.drop').removeClass('drop');


							var helper = $(ui.helper).parent().find('.ui-sortable-helper');
							var placeholder = $(ui.helper).parent().find('.ui-sortable-placeholder');
							var lastIndex = $(ui.helper).parent().children().length-1;




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
							//let alias = $scope.areas[ui.item.sortable.index].contentData.contentTypeAlias;
							//let dropAllowedElements = $scope.areas[ui.item.sortable.dropindex].allowedElementTypes.split(',');
							//let dropAllowedContent = $scope.areas[ui.item.sortable.dropindex].allowedContentTypes.split(',');
							//let allowed = false;

							//allowed = _.contains(dropAllowedContent, alias) || _.contains(dropAllowedElements, alias);

							//if (!allowed) {
							//	notificationsService.error("Unable to move item.  It is not allowed in the new position");
							//	ui.item.sortable.cancel();
							//}
						}
						,
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
					if ($scope.config.useCssFile && $scope.config.cssFilePath) {
						let url = '/umbraco/backoffice/Api/Bento/LoadEmbeddedStyler';

						let data = {
							guid: guid(),
							contentTypeAlias: $scope.settings.contentTypeAlias,
							dataJson: JSON.stringify($scope.settings),
							culture: typeof $routeParams.mculture !== 'undefined' ? $routeParams.mculture : null
						};

						$http.post(url, data).then(function (response) {

							$scope.getStyler = function () {
								return $sce.trustAsHtml(response.data);
							};

						}).catch(function (error) {
							console.log(error);

						}).finally(function () {
							//$scope.updating = false;
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

				function toggleDeleteConfirm(area, show) {
					area.deleteConfirmVisible = show;
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

					$scope.updating = true;
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


				$scope.$watch('areas', function (newValue, oldValue) {

					$scope.updating = true;

				}, true);

				$scope.$watch('layout', function (newValue, oldValue) {

					//reinit layout
					$scope.initLayout();
					$scope.updating = true;

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