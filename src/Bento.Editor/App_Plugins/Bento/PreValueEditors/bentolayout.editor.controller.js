function bentoLayoutEditorController($scope, angularHelper, notificationsService, editorService) {
	var vm = this;

	vm.renderModel = [];
	vm.remove = remove;
	vm.openLayoutEditor = openLayoutEditor;

	function remove(index) {
		vm.renderModel.splice(index, 1);
		vm.currentForm.$setDirty();
	}

	function openLayoutEditor(layout, index) {
		var renderModel = vm.renderModel;
		var currentForm = vm.currentForm;

		var options = {
			view: "/App_Plugins/Bento/prevalueeditors/bentolayout.edit.html",
			layout: layout,
			size: "medium",
			submit: function (model, hasChanged) {
				if (model === null) {
					notificationsService.error("No layout returned");
					return;
				}

				//if (!hasChanged) {
				//	return;
				//}

				if (typeof index !== "undefined" && index !== null && index >= 0 && index < renderModel.length) {
					renderModel[index] = model;
				}
				else {
					renderModel.push(model);
				}

				currentForm.$setDirty();
				editorService.close();
			},
			close: function () {
				editorService.close();
			}
		};

		editorService.open(options);
	}

	function formSubmitting() {
		var submitModel = vm.renderModel;

		if (!submitModel || submitModel.length < 1) {
			submitModel = [];
		}

		$scope.model.value = JSON.stringify(submitModel);
	}

	function init() {
		if ($scope.preview) {
			return;
		}

		if ($scope.model.value) {
			var layoutArray = JSON.parse($scope.model.value);

			layoutArray.forEach(function (layout) {
				vm.renderModel.push(layout);
			});
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

angular.module("umbraco").controller("bento.layout.editor.controller", bentoLayoutEditorController);