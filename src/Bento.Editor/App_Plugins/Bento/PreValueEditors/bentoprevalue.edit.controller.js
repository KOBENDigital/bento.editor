function bentoPreValueEditController($scope, notificationsService) {
	var vm = this;

	vm.close = close;
	vm.submit = submit;
	vm.errors = [];
	
	function init() {
		vm.preValue = $scope.model.preValue;

		if (!vm.preValue) {
			vm.preValue = {
				label: null,
				value: null
			}
		}
	}

	function close() {
		if ($scope.model.close) {
			$scope.model.close();
		}
	}

	function submit() {
		vm.errors = [];

		if (!vm.preValue.label || !/\S/.test(vm.preValue.label)) {
			vm.errors.push("Label is required");
			notificationsService.error("Failed to save pre value", "<br/>" + vm.errors.join("<br/>"));
			return;
		}

		if (!vm.preValue.value) {
			vm.preValue.value = "";
		}
		
		if ($scope.model.submit) {
			$scope.model.submit(vm.preValue);
		}
	}

	init();
}

angular.module("umbraco").controller("bento.prevalue.edit.controller", bentoPreValueEditController);

