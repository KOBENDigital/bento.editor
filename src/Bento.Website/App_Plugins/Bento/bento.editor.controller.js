(function () {
	'use strict';

	function bentoEditorController($scope,$sce, assetsService) {

		var vm = this;

		vm.remove = remove;
		vm.toggleDeleteConfirm = toggleDeleteConfirm;
		vm.canAddContent = canAddContent;
		vm.canAddMultiple = canAddMultiple;
		vm.updating = false;
		vm.deleteConfirmVisible = false;

		vm.item = {
			name: "...",
			icon: "icon-brick",
			contents: []
		};
		//vm.item.id = undefined;
		//vm.item.key = undefined;
		//vm.item.contentData = undefined;
		//vm.contentNode = undefined;
		vm.config = $scope.model.config;


		if ($scope.model.config.useCssFile && $scope.model.config.fontCssUrls) {
			vm.cssFonts = $scope.model.config.fontCssUrls.split('*');
		}

		vm.trustSrc = function (src) {
			return $sce.trustAsResourceUrl(src);
		};

		$scope.model.hideLabel = $scope.model.config.hideLabel;

		if ($scope.model.value) {
			/// old bento hacks
			var data = {};

			if ($scope.model.value.contents !== undefined) {
				data = $scope.model.value;
			} else if ($scope.model.value.id !== undefined) {
				data = $scope.model.value;
			} else {
				// some old version of Bento?
				data.id = $scope.model.value;
				data.key = undefined;
				data.contentData = undefined;
				data.icon = undefined;
			}
			///

			//vm.item.id = data.id;
			//vm.item.key = data.key;
			//vm.item.contentData = data.contentData;
			vm.item.icon = data.icon;

			if (data.contents !== undefined) {
				vm.item.contents = data.contents;
			} else {
				if (canAddMultiple())
					console.log('data');
				vm.item.contents = [];
				if (data.contentData || data.id || data.key) { // if any of these things were set by $scope.model.value (should be unset by next save)
					vm.item.contents.push({ id: data.id, name: data.name, alias: data.alias, key: data.key, contentData: data.contentData, icon: data.icon, contentNode: data.contentNode });
				}
			}
		}

		function validate() {
			if (canAddMultiple()) {
				if (vm.item.contents.length < $scope.model.config.minNumber) {
					$scope.bentoEditorForm.minCount.$setValidity("minCount", false);
				} else {
					$scope.bentoEditorForm.minCount.$setValidity("minCount", true);
				}
				if (vm.item.contents.length > $scope.model.config.maxNumber) {
					$scope.bentoEditorForm.maxCount.$setValidity("maxCount", false);
				} else {
					$scope.bentoEditorForm.maxCount.$setValidity("maxCount", true);
				}
			}
		}

		$scope.$on("formSubmitting", function () {
			validate();
			if (canAddMultiple()) {
				//save multiple
				$scope.model.value = JSON.stringify(vm.item);
			} else {
				//save singular
				var data = vm.item.contents[0];
				if (data !== undefined && (data.id !== undefined || data.key !== undefined)) {
					data.icon = vm.item.icon;
					$scope.model.value = JSON.stringify(data);
				}
			}
		});

		/*$scope.$watch('vm.updating', function (newValue, oldValue) {
			console.log('vm.updating');
			vm.updating = false;
		});*/

		function toggleDeleteConfirm(show) {
			vm.deleteConfirmVisible = show;
		}

		function canAddContent() {
			if (canAddMultiple()) {
				//allow multiple entries (depending on configuration)
				return vm.item.contents.length < $scope.model.config.maxNumber;
			}
			return vm.item.contents.length === 0; // allow single entry
		}

		//Use to check that this is a Multi Items by checking for config.minNumber/config.maxNumber 
		function canAddMultiple() {
			return ($scope.model.config.minNumber !== undefined && $scope.model.config.maxNumber !== undefined);
		}

		function remove() {
			vm.item = {};
			vm.item.id = undefined;
			vm.item.key = undefined;
			vm.item.contentData = undefined;
			vm.contentNode = undefined;

			vm.name = "...";
			vm.item.icon = "icon-brick";
			$scope.model.value = undefined;

			vm.deleteConfirmVisible = false;
		}
	}

	angular.module('umbraco').controller('bento.editor.controller', bentoEditorController);
})();