function bentoSettingsEditorController($scope, angularHelper, editorService, notificationsService, bentoSettingFactory) {
	var vm = this;

	vm.settings = [];
	vm.remove = remove;
	vm.openSettingEditor = openSettingEditor;

	function remove(index) {
		vm.settings.splice(index, 1);
		vm.currentForm.$setDirty();
	};

	function openSettingEditor(setting, index) {
		var settings = vm.settings;
		var currentForm = vm.currentForm;

		var options = {
			view: "/App_Plugins/Bento/prevalueeditors/bentosetting.edit.html",
			setting: setting,
			size: "small",
			submit: function (model, hasChanged) {
				if (!model) {
					notificationsService.error("No setting returned");
					return;
				}

				if (!hasChanged) {
					return;
				}

				if (typeof index !== "undefined" && index !== null && index >= 0 && index < settings.length) {
					settings[index] = model;
				}
				else {
					settings.push(model);
				}

				currentForm.$setDirty();
				notificationsService.info("Setting: '" + model.alias + "' has been updated", "<br/>Don't forget to save your updates!");
				editorService.close();
			},
			close: function () {
				editorService.close();
			}
		};

		editorService.open(options);
	};

	function formSubmitting() {
		var errors = [];

		var submitModel = bentoSettingFactory.buildSettingsForSave(vm.settings, errors, true);

		if (errors.length > 0) {
			notificationsService.error("Failed to save settings", "<br/>" + errors.join("<br/>"));
			return;
		}

		$scope.model.value = JSON.stringify(submitModel);
	}

	function init() {
		if ($scope.model.value) {
			var settingsObject = JSON.parse($scope.model.value);
			vm.settings = bentoSettingFactory.convertSettingsObjectToArray(settingsObject, null);
		}

		if ($scope.preview) {
			return;
		}

		vm.currentForm = angularHelper.getCurrentForm($scope);

		vm.sortableOptions = {
			distance: 10,
			tolerance: "pointer",
			scroll: true,
			zIndex: 6000,
			update: function () {
				vm.currentForm.$setDirty();
			}
		};

		$scope.$on("formSubmitting", formSubmitting);
	}

	init();
}

angular.module("umbraco").controller("bento.settings.editor.controller", bentoSettingsEditorController);

