function bentoLayoutEditController($scope, notificationsService, localizationService, editorService) {

	var vm = this;

	vm.areaEdit = false;
	vm.changes = false;

	vm.removeArea = removeArea;
	vm.addColumn = addColumn;
	vm.removeColumn = removeColumn;
	vm.close = close;
	vm.submit = submit;
	vm.confirmClose = confirmClose;
	vm.openCloseOverlay = openCloseOverlay;
	vm.layoutChanged = layoutChanged;
	vm.openAreaEditor = openAreaEditor;
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

	function addColumn() {
		if (!vm.newCol || vm.newCol === "") return;
		vm.model.columns.push(vm.newCol +'fr');
		vm.newCol = "";
		vm.changes = true;
	}

	function removeColumn(index) {
		vm.model.columns.splice(index, 1);
		vm.changes = true;
	}


	function openAreaEditor(area, index) {
		var renderModel = vm.model.areas;

		var options = {
			view: "/App_Plugins/Bento/prevalueeditors/bentolayoutarea.edit.html",
			area: area,
			allowSort: vm.model.allowSort.value,
			size: "small",
			submit: function (model, hasChanged) {
				if (model === null) {
					notificationsService.error("No area returned");
					return;
				}

				//if (!hasChanged) {
				//	editorService.close();
				//	return;
				//}

				if (typeof index !== "undefined" && index !== null && index >= 0 && index < renderModel.length) {
					renderModel[index] = model;
				}
				else {
					renderModel.push(model);
				}

				editorService.close();
			},
			close: function () {
				editorService.close();
			}
		};

		editorService.open(options);
	}



	


	function removeArea(index) {
		vm.model.areas.splice(index, 1);
		vm.changes = true;
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

	
		vm.errors = [];

		if ($scope.model.submit) {
			if (!vm.model.alias || !/\S/.test(vm.model.alias)) {
				vm.errors.push("Alias is required");
			}

			if (!vm.model.name || !/\S/.test(vm.model.name)) {
				vm.errors.push("Name is required");
			}

			if (!vm.model.columns || vm.model.columns.length < 1) {
				vm.errors.push("Columns are required");
			}

			if (!vm.model.areas || vm.model.areas.length < 1) {
				vm.errors.push("Areas are required");
			}

			if (vm.errors.length > 0) {
				notificationsService.error("Failed to save layout", "<br/>" + vm.errors.join("<br/>"));
				return;
			}

			// convert from picker model to values

			var layoutSettings = vm.model.layoutSettings.value;
			vm.model.layoutSettings = layoutSettings;

			var allowSort = vm.model.allowSort.value;
			vm.model.allowSort = allowSort;

			if (allowSort) {

				//if sort is allowed all elements need the smae allowed items

				var allowedElementTypes = vm.model.allowedElementTypes.value;
				vm.model.allowedElementTypes = allowedElementTypes;
				var allowedContentTypes = vm.model.allowedContentTypes.value;
				vm.model.allowedContentTypes = allowedContentTypes;

				angular.forEach(vm.model.areas, function (area){
					area.allowedElementTypes = allowedElementTypes;
					area.allowedContentTypes = allowedContentTypes;
				});


			} else {

				if (typeof vm.model.allowedElementTypes === 'object') {
					vm.model.allowedElementTypes = '';
				}
				if (typeof vm.model.allowedContentTypes === 'object') {
					vm.model.allowedContentTypes = '';
				}
			}


			$scope.model.submit(vm.model, vm.changes);
		}
	}

	function setupEmptyModel() {
		vm.model = {
			alias: null,
			name: null,
			columns: [],
			areas: [],
			layoutSettings: {

				view: '/app_plugins/bento/views/prevalueeditors/singlerequiredcontenttypepicker.html',
				value: '',
				updating: true,
				config: {}

			}
			,
			allowSort: {
				view: '/umbraco/views/prevalueeditors/boolean.html',
				value: false
			},
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


	function init() {
		setupEmptyModel();
		if ($scope.model.layout){
			var data = JSON.parse(JSON.stringify($scope.model.layout));

			vm.model.name = data.name;
			vm.model.alias = data.alias;
			vm.model.columns = data.columns;
			vm.model.areas = data.areas;
			vm.model.layoutSettings.value = data.layoutSettings !== undefined ? data.layoutSettings : '';

			vm.model.allowedContentTypes.value = data.allowedContentTypes !== undefined ? data.allowedContentTypes : '';
			vm.model.allowedElementTypes.value = data.allowedElementTypes !== undefined ? data.allowedElementTypes : '';
			vm.model.allowSort.value = data.allowSort !== undefined ? data.allowSort : false;
		}
	}

	init();
}

angular.module("umbraco").controller("bentolayout.edit.controller", bentoLayoutEditController);