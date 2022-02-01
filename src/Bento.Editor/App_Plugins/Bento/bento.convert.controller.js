(function () {
	'use strict';

	function bentoConvertController($scope, $routeParams, editorService, bentoResource) {

		var vm = this;

		vm.close = close;
		vm.submit = submit;
		vm.convert = convert;
		vm.libraryFolderId = 0;
		vm.itemTypeFolderId = 0;
		vm.doctypes = [];
		vm.clickItem = clickItem;
		vm.selectedTypeAlias = '';
		vm.selectedTypeName = '';
		vm.itemid = $scope.model.itemid;

		function init() {

			bentoResource.getAllowedElementTypes($scope.model.config.allowedElementAliases)
			.then(function (ent) {
				return vm.doctypes = ent;
			});

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
				vm.convert();
				//$scope.model.submit($scope.model);
			}
		}

		function convert() {

			/// if we are going to embed this is where the process is kicked off.

				let options = {
					title: 'Convert',

					itemid: vm.itemid,
					documentTypeAlias: vm.selectedTypeAlias,
					documentTypeName: vm.selectedTypeName,

					view: '/App_Plugins/Bento/bento.convert.map.html',
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

		init();
	}

	angular.module('umbraco').controller('bento.convert.controller', bentoConvertController);
})();
