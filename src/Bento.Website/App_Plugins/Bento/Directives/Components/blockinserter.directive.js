(function () {
	'use strict';

	function bentoBlockInserterDirective(editorService, bentoResource, contentResource, userService, $routeParams, $sce, $http) {

		var directive = {
			restrict: 'E',
			templateUrl: '/App_Plugins/Bento/Views/Components/bento-block-inserter.html',
			scope: {
				id: '=',
				config: '=',
				area: '=',
				updating: '='
			},

			controller: function ($scope, $element) {

				$scope.create = create;
				$scope.useExisting = useExisting;
				$scope.hasElements = hasElements;
				$scope.hasLibrary = hasLibrary;
				$scope.adminOnly = false;

				$scope.allowedContentTypes = "";
				$scope.allowedElementTypes = "";

				//hack to find out if user is admin
				userService.getCurrentUser().then(function (userObj) {
					var user = userObj;
					$scope.adminOnly = user.userGroups.indexOf("admin") >= 0;
				});

				// Has checks

				function hasElements() {
					if ($scope.allowedElementTypes) {
						return true;
					}
					return false;
				}

				function hasLibrary() {
					if ($scope.allowedContentTypes) {
						return true;
					}
					return false;
				}

				// Helpers

				// Handlers

				function create(embed) {
					var options;

					options = {
						title: 'Bento block',
						view: '/App_Plugins/Bento/bento.create.html',
						embed: embed,
						config: $scope.config,
						area: $scope.area,
						submit: function (model) {
							var newItem = {};
							if (model.embed) {
								var value = bentoResource.buildEmbeddedContentData(model.node);
								newItem.contentData = value; // this is what we will save
								newItem.contentNode = model.node;
							} else {
								newItem.contentNode = model.contentNode;
							}
							newItem.id = newItem.contentNode.id;
							newItem.key = newItem.contentNode.key;
							$scope.area.contents.push(newItem);
							$scope.updating = true;
						},
						close: function (model) {
							editorService.close();
						}
					};

					editorService.open(options);
				}

				function useExisting() {
					bentoResource.getLibraryFolderId($routeParams.id, $scope.config.libraryFolderDoctypeAlias)
						.then(function (ent) {
							var libraryFolderId = ent;
							var contentPicker = {
								section: "content",
								treeAlias: "content",
								multiPicker: false,
								startNodeId: libraryFolderId,
								filterCssClass: 'not-allowed not-published',
								filter: $scope.allowedContentTypes,
								submit: function (model) {
									var newItem = {
										contentNode: model.selection[0],
									};
									newItem.id = newItem.contentNode.id;
									newItem.key = newItem.contentNode.key;
									$scope.area.contents.push(newItem);

									$scope.updating = true;
									editorService.close();
								},
								close: function () {
									editorService.close();
								}
							};

							editorService.treePicker(contentPicker);
						});
				}

				// Initers

				function init() {
					//set bento item allowed blocks
					if ($scope.config !== undefined) {
						if ($scope.config.allowedDoctypeAliases) {
							$scope.allowedContentTypes = $scope.config.allowedDoctypeAliases;
						}
						if ($scope.config.allowedElementAliases) {
							$scope.allowedElementTypes = $scope.config.allowedElementAliases;
						}
					}

					//set bento stack allowed blocks
					if ($scope.area !== undefined) {
						// this could probably be done better
						if ($scope.area.allowedElementTypes !== undefined) {
							if ($scope.area.allowedElementTypes.value !== undefined && $scope.area.allowedElementTypes.value !== '') {
								$scope.allowedElementTypes = $scope.area.allowedElementTypes.value;
							}
							if (typeof $scope.area.allowedElementTypes === 'string' && $scope.area.allowedElementTypes) {
								$scope.allowedElementTypes = $scope.area.allowedElementTypes;
							}
						}

						if ($scope.area.allowedContentTypes !== undefined) {
							if ($scope.area.allowedContentTypes.value !== undefined && $scope.area.allowedContentTypes.value !== '') {
								$scope.allowedContentTypes = $scope.area.allowedContentTypes.value;
							}
							if (typeof $scope.area.allowedContentTypes === 'string' && $scope.area.allowedContentTypes) {
								$scope.allowedContentTypes = $scope.area.allowedContentTypes;
							}
						}
					}
				}

				init();
			}
		}

		return directive;
	};

	angular.module('umbraco.directives').directive('bentoBlockInserter', bentoBlockInserterDirective);
})();