(function () {
	"use strict";

	function bentoCopyController($scope) {

		var vm = this;

		vm.errors = [];
		vm.submit = submit;
		vm.close = close;
		vm.clipName = null;

		function init() {

		}

		function clipNameChanged() {

		}

		function submit() {
			vm.errors = [];

			if (!vm.clipName || !/\S/.test(vm.clipName)) {
				vm.errors.push("Name is required");
			}

			if (vm.errors.length > 0) {
				return;
			}

			if ($scope.model.submit) {
				$scope.model.submit(vm.clipName);
			}
		}

		function close() {
			if ($scope.model.close) {
				$scope.model.close();
			}
		}

		init();
	}

	angular.module("umbraco").controller("bento.copy.controller", bentoCopyController);
})();