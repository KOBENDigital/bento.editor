(function() {
	"use strict";

	function bentoStackSettingsEditorController($scope, notificationsService, bentoSettingFactory) {

		var vm = this;

		vm.errors = [];
		vm.close = close;
		vm.submit = submit;
		vm.settings = [];

		function init() {
			vm.settings = bentoSettingFactory.convertSettingsObjectToArray($scope.model.defaultSettings, $scope.model.blockSettings);
		}

		function close() {
			if ($scope.model.close) {
				$scope.model.close();
			}
		}

		function submit() {
			vm.errors = [];

			if ($scope.model.submit) {
				var submitModel = bentoSettingFactory.buildSettingsForSave(vm.settings, vm.errors);
				
				if (vm.errors.length > 0) {
					notificationsService.error("Failed to save settings", "<br/>" + vm.errors.join("<br/>"));
					return;
				}

				if (submitModel === null) {
					return;
				}

				$scope.model.submit(submitModel);
			}
		}

		init();
	}

	angular.module("umbraco").controller("bento.stack.settings.editor.controller", bentoStackSettingsEditorController);
})();