function bentoLayoutAreaEditController($scope, notificationsService, localizationService) {

	var vm = this;

	vm.changes = false;

	vm.saveArea = saveArea;
	vm.close = close;
	vm.submit = submit;
	vm.confirmClose = confirmClose;
	vm.openCloseOverlay = openCloseOverlay;
	vm.layoutChanged = layoutChanged;
	vm.errors = [];

	function layoutChanged() {
		vm.changes = true;
	}

	function openCloseOverlay() {
		if (!vm.changes) {
			vm.close();
			return;
		}

		localizationService.localizeMany(["bento_unsavedLayoutChangeWarningTitle", "bento_unsavedLayoutChangeWarningSubtitle", "bento_cancel", "bento_imOkWithThis"]).then(function (data) {

			var title = data[0];
			var subTitle = data[1];
			var cancel = data[2];
			var imOkWithThis = data[3];
			vm.closeOverlay = {
				title: title,
				subtitle: subTitle,
				closeButtonLabel: cancel,
				submitButtonLabel: imOkWithThis,
				show: true,
				submit: function (model) {
					vm.closeOverlay.show = false;
					vm.closeOverlay = null;
					vm.close();
				},
				close: function (oldModel) {
					vm.closeOverlay.show = false;
					vm.closeOverlay = null;
				}
			};
		});
	}


	function saveArea() {

		if (!vm.area.name || !vm.area.alias || !vm.area.colLine1 || !vm.area.rowLine1) return;

		vm.model = {};

		vm.model.name = vm.area.name;
		vm.model.alias = vm.area.alias;

		vm.model.column = vm.area.colLine1;
		if (vm.area.colLine2) {
			vm.model.column += "/" + vm.area.colLine2;
		}

		vm.model.row = vm.area.rowLine1;
		if (vm.area.rowLine2) {
			vm.model.row += "/" + vm.area.rowLine2;
		}

		vm.model.allowedElementTypes = vm.area.allowedElementTypes.value;
		vm.model.allowedContentTypes = vm.area.allowedContentTypes.value;

	}


	function setupEmptyArea() {
		vm.area = {
			name: "",
			alias: "",
			colLine1: undefined,
			colLine2: undefined,
			rowLine1: undefined,
			rowLine2: undefined,
			allowedElementTypes: {

				view: '/app_plugins/bento/views/prevalueeditors/contenttypepicker.html',
				value: '',
				updating: true,
				config: {}

			},
			allowedContentTypes: {

				view: '/app_plugins/bento/views/prevalueeditors/contenttypepicker.html',
				value: '',
				updating: true,
				config: {}

			}
		};
	}



	function close() {
		if ($scope.model.close) {
			$scope.model.close();
		}
	}

	function confirmClose() {
		openCloseOverlay();
	}

	function submit() {

		saveArea();

		vm.errors = [];

		if ($scope.model.submit) {
			if (!vm.model.alias || !/\S/.test(vm.model.alias)) {
				vm.errors.push("Area alias is required");
			}

			if (!vm.model.name || !/\S/.test(vm.model.name)) {
				vm.errors.push("Area name is required");
			}

			if (!vm.model.column || !/\S/.test(vm.model.column)) {
				vm.errors.push("Column details required");
			}

			if (!vm.model.row || !/\S/.test(vm.model.row)) {
				vm.errors.push("Row details required");
			}

			if (vm.errors.length > 0) {
				notificationsService.error("Failed to save layout", "<br/>" + vm.errors.join("<br/>"));
				return;
			}

			$scope.model.submit(vm.model, vm.changes);
		}
	}

	function init() {
		setupEmptyArea();

		vm.allowSort = Boolean(Number($scope.model.allowSort));

		if ($scope.model.area) {
			
			var data = JSON.parse(JSON.stringify($scope.model.area));

			var colLineSplit = data.column.toString().split("/");

			vm.area.name = data.name;
			vm.area.alias = data.alias;

			vm.area.colLine1 = parseInt(colLineSplit[0]);
			vm.area.colLine2 = colLineSplit[1] ? parseInt(colLineSplit[1]) : undefined;

			var rowLineSplit = data.row.toString().split("/");

			vm.area.rowLine1 = parseInt(rowLineSplit[0]);
			vm.area.rowLine2 = rowLineSplit[1] ? parseInt(rowLineSplit[1]) : undefined;

			vm.area.allowedElementTypes.value = data.allowedElementTypes !== undefined ? data.allowedElementTypes : '';
			vm.area.allowedContentTypes.value = data.allowedContentTypes !== undefined ? data.allowedContentTypes : '';

		}
	}

	init();
}

angular.module("umbraco").controller("bentolayoutarea.edit.controller", bentoLayoutAreaEditController);