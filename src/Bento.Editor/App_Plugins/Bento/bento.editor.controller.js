﻿(function () {
	'use strict';

	angular.module('umbraco').run(['clipboardService', function (clipboardService) {

		function resolveBentoItemForPaste(prop, propClearingMethod) {

			// if we got an array, and it has a entry with ncContentTypeAlias this means that we are dealing with a NestedContent property data.
			if ((Array.isArray(prop) && prop.length > 0 && prop[0].ncContentTypeAlias !== undefined)) {

				for (var i = 0; i < prop.length; i++) {
					var obj = prop[i];

					// generate a new key.
					obj.key = String.CreateGuid();

					// Loop through all inner properties:
					for (var k in obj) {
						propClearingMethod(obj[k], clipboardService.TYPES.RAW);
					}
				}
			}
		}

		clipboardService.registerPastePropertyResolver(resolveBentoItemForPaste, clipboardService.TYPES.RAW)
	}]);

	function bentoEditorController($scope, $sce, assetsService, clipboardService, overlayService, editorState) {


		var vm = this;

		vm.remove = remove;
		vm.toggleDeleteConfirm = toggleDeleteConfirm;
		vm.deleteConfirmVisible = false;

		vm.label = $scope.$parent.model.label;
		vm.item = {};
		vm.item.id = undefined;
		vm.item.key = undefined;
		vm.item.contentData = undefined;
		vm.contentNode = undefined;
		vm.config = $scope.model.config;

		//setup the clipboard service
		clipboardService.registrerTypeResolvers();

		if ($scope.model.config.usePreviewJs && $scope.model.config.jsFilePath && $scope.model.config.jsFilePath !== null && $scope.model.config.jsFilePath !== '') {
			//todo: we're replacing the 'wwwroot' that gets appended in v9/core... not ideal, we're looking into getting the tree picker to start in the wwwroot https://our.umbraco.com/forum/using-umbraco-and-getting-started//108099-is-it-possible-to-start-the-editor-service-file-picker-in-the-wwwroot
			//console.log('loading site js');
			assetsService.loadJs($scope.model.config.jsFilePath.replace('/wwwroot', ''), $scope).then(function () { });
		}

		if ($scope.model.config.useCssFile && $scope.model.config.fontCssUrls) {
			vm.cssFonts = $scope.model.config.fontCssUrls.split('*');
		}

		vm.trustSrc = function (src) {
			return $sce.trustAsResourceUrl(src);
		};

		vm.name = "...";
		vm.item.icon = "icon-brick";

		$scope.model.hideLabel = $scope.model.config.hideLabel;

		function updatePropertyActionStates() {

			var hasItemToPaste = clipboardService.hasEntriesOfType(clipboardService.TYPES.RAW, [$scope.umbProperty.property.dataTypeKey]);

			copyItemAction.isDisabled = !$scope.model.value;
			pasteItemAction.isDisabled = !hasItemToPaste;
		}

		var copyItem = function () {
			clipboardService.copy(clipboardService.TYPES.RAW, $scope.umbProperty.property.dataTypeKey, $scope.model.value, `${$scope.umbProperty.property.alias} from ${editorState.current.variants[0].name}`, "icon-indent", vm.item.key);
			updatePropertyActionStates();
		}

		var pasteFromClipboard = function (data) {
			vm.item = JSON.parse(data);
			$scope.model.value = vm.item;
			$scope.$broadcast("bentoSyncVal", vm.item);
		}

		var pasteItem = function () {

			const dialog = {
				orderBy: "$index",
				view: "itempicker",

				size: "small",
				clickPasteItem: function (item) {
					pasteFromClipboard(item.data);
					overlayService.close();
				},
				close: function () {
					overlayService.close();
				}
			};

			dialog.pasteItems = [];
			dialog.availableItems = [];

			var entriesForPaste = clipboardService.retriveEntriesOfType(clipboardService.TYPES.RAW, [$scope.umbProperty.property.dataTypeKey]);
			_.each(entriesForPaste, function (entry) {
				dialog.pasteItems.push({
					date: entry.date,
					name: entry.label,
					data: entry.data,
					icon: entry.icon
				});
			});

			dialog.pasteItems.sort((a, b) => {
				return b.date - a.date;
			});

			dialog.hideHeader = true;

			dialog.clickClearPaste = function ($event) {
				$event.stopPropagation();
				$event.preventDefault();
				clipboardService.clearEntriesOfType(clipboardService.TYPES.RAW, [$scope.umbProperty.property.dataTypeKey]);
				dialog.pasteItems = [];// This dialog is not connected via the clipboardService events, so we need to update manually.
				dialog.hideHeader = false;
			};

			overlayService.open(dialog);
		}

		var copyItemAction = {
			labelKey: 'clipboard_labelForCopyAllEntries',
			labelTokens: [vm.label],
			icon: 'documents',
			method: copyItem,
			isDisabled: true
		};

		var pasteItemAction = {
			labelKey: 'content_createFromClipboard',
			labelTokens: [vm.label],
			icon: 'color-bucket',
			method: pasteItem,
			isDisabled: true
		}

		var propertyActions = [
			copyItemAction,
			pasteItemAction
		];

		var unsubscribe = $scope.$on("bentoSyncVal", function (ev, args) {
			if (args.id !== undefined || args.key !== undefined) {
				$scope.model.value = JSON.stringify(args);
				updatePropertyActionStates();
			}
		});

		$scope.$on('$destroy', function () {
			unsubscribe();
		});
		
		function toggleDeleteConfirm(show) {
			vm.deleteConfirmVisible = show;
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

		var init = function () {
			if ($scope.model.value) {
				/// old bento hacks
				var data = {};

				var jsonObj = {};

				//check if $scope.model.value iS JSON or not
				if (typeof $scope.model.value === "string") {
					jsonObj = JSON.parse($scope.model.value);
				} else {
					jsonObj = $scope.model.value;
				}

				if (jsonObj.id !== undefined) {
					data = jsonObj;
				} else {
					data.id = jsonObj;
					data.key = undefined;
					data.contentData = undefined;
					data.icon = undefined;
				}
				///

				vm.item.id = data.id;
				vm.item.key = data.key;
				vm.item.contentData = data.contentData;
				vm.item.icon = data.icon;

				$scope.$broadcast("bentoSyncVal", vm.item);
			}
		}

		this.$onInit = function () {
			if ($scope.umbProperty) {
				$scope.umbProperty.setPropertyActions(propertyActions);
			}

			init();

			// you are probably in the doctype editor so we have no umbProperty
			if ($scope.umbProperty == null) {
				return;
			}

			if ($scope.umbProperty.property) {
				updatePropertyActionStates();
			}
		};
	}

	angular.module('umbraco').controller('bento.editor.controller', bentoEditorController);
})();