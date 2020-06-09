(function () {
	'use strict';

	function bentoCreateController($scope, $routeParams, editorService, bentoResource) {

		var vm = this;

		vm.close = close;
		vm.submit = submit;
		vm.create = create;
		vm.libraryFolderId = 0;
		vm.itemTypeFolderId = 0;
		vm.doctypes = [];
		vm.clickItem = clickItem;
		vm.selectedTypeAlias = '';
		vm.selectedTypeName = '';
		vm.embed = $scope.model.embed;
		vm.allowedContentTypes = $scope.model.area !== undefined ? $scope.model.area.allowedContentTypes : $scope.model.config.allowedDoctypeAliases;
		vm.allowedElementTypes = $scope.model.area !== undefined ? $scope.model.area.allowedElementTypes : $scope.model.config.allowedElementAliases;

		function init() {

			if (vm.embed) {
				bentoResource.getAllowedElementTypes(vm.allowedElementTypes)
					.then(function (ent) {
						return vm.doctypes = ent;
					});

			} else {

				bentoResource.getLibraryFolderId($routeParams.id, $scope.model.config.libraryFolderDoctypeAlias)
					.then(function (ent) {
						vm.libraryFolderId = ent;
						bentoResource.getAllowedContentTypes(vm.allowedContentTypes)
							.then(function (ent) {
								return vm.doctypes = ent;
							});
					});



			}
		}

		function clickItem(item, $event, $index) {

			angular.forEach(vm.doctypes, function (value, key) {
				value.selected = value.alias === item.alias ? true : false;
			});

			vm.selectedTypeAlias = item.alias;
			vm.selectedTypeName = item.name;
			item.selected = true;
		}

		function close() {
			if ($scope.model.close) {
				$scope.model.close();
			}
		}

		function submit() {
			if ($scope.model.submit) {
				vm.create();
				//$scope.model.submit($scope.model);
			}
		}

		function create() {

			/// if we are going to embed this is where the process is kicked off.
			if (vm.embed) {
				let options = {
					title: 'Edit',
					embed: vm.embed,
					documentTypeAlias: vm.selectedTypeAlias,
					documentTypeName: vm.selectedTypeName,
					view: '/App_Plugins/Bento/bento.edit.html',
					submit: function (model) {
						if (model.submit) {
							$scope.model.submit(model);
						}

						angular.forEach(vm.doctypes, function (value, key) {
							value.selected = false;
						});

						editorService.close();
						close();
					},
					close: function (model) {
						editorService.close();
					}
				};

				editorService.open(options);
			}

			/// else

			else {

				bentoResource.getItemTypeFolderId(vm.libraryFolderId, $scope.model.config.itemTypeFolderDoctypeAlias, vm.selectedTypeName)
					.then(function (ent) {

						var options = {
							create: true,
							documentTypeAlias: vm.selectedTypeAlias,
							parentId: ent,
							allowSaveAndClose: true,
							allowPublishAndClose: true,

							submit: function (model) {
								if (model.submit) {
									$scope.model.submit(model);
								}

								angular.forEach(vm.doctypes, function (value, key) {
									value.selected = false;
								});

								editorService.close();
								close();
							},
							close: function (model) {
								editorService.close();
							}
						};

						editorService.contentEditor(options);

					});
			}
			///
		}

		init();
	}

	angular.module('umbraco').controller('bento.create.controller', bentoCreateController);
})();
