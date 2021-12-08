function bentoSettingEditController($scope, notificationsService) {
	var vm = this;

	vm.errors = [];
	vm.createMode = false;
	vm.changes = false;
	vm.submit = submit;
	vm.confirmClose = confirmClose;
	vm.settingChanged = settingChanged;

	function settingChanged() {
		vm.changes = true;
	}

	function confirmClose() {
		openCloseOverlay();
	}

	function openCloseOverlay() {
		if (!vm.changes) {
			close();
			return;
		}

		vm.closeOverlay = {
			title: "You have made changes",
			subtitle: "If you continue all changes to " + vm.setting.label + ". If you close all changes you have made to this setting will be lost",
			closeButtonLabel: "keep editing",
			submitButtonLabel: "Close anyway",
			show: true,
			submit: function () {
				vm.closeOverlay.show = false;
				vm.closeOverlay = null;
				close();
			},
			close: function () {
				vm.closeOverlay.show = false;
				vm.closeOverlay = null;
			}
		};
	}

	function submit() {
		vm.errors = [];

		if ($scope.model.submit) {
			if (!vm.setting.alias || !/\S/.test(vm.setting.alias)) {
				vm.errors.push("Alias is required");
			}

			if (!vm.setting.label || !/\S/.test(vm.setting.label)) {
				vm.errors.push("Label is required");
			}

			if (!vm.setting.type || !/\S/.test(vm.setting.type)) {
				vm.errors.push("Type is required");
			}

			if (vm.errors.length > 0) {
				notificationsService.error("Failed to save setting", "<br/>" + vm.errors.join("<br/>"));
				return;
			}

			$scope.model.submit(vm.setting, vm.changes);
			return;
		}

		return;
	}

	function close() {
		if ($scope.model.close) {
			$scope.model.close();
		}
	}
	
	function init() {
		vm.settingTypes = ["string", "text", "boolean", "radio", "checklist", "dropdown"];
		
		if (!$scope.model.setting) {
			vm.setting = {
				type: vm.settingTypes[0]
			}
			vm.createMode = true;
		} else {
			vm.setting = JSON.parse(JSON.stringify($scope.model.setting));
		}
	}

	init();
}

angular.module("umbraco").controller("bento.setting.edit.controller", bentoSettingEditController);

