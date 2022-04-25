(function() {
	"use strict";

	function bentoLayoutsEditorController($scope, localizationService, overlayService, clipboardService) {

		var vm = this;

		vm.errors = [];
		vm.layoutSelected = layoutSelected;
		vm.clipboardSelected = clipboardSelected;
		vm.submit = submit;
		vm.close = close;
		vm.layouts = [];
		vm.clipboard = [];
		vm.selectedLayout = null;
		vm.clipSelected = null;
		vm.getLayoutStyle = getLayoutStyle;
		vm.getAreaStyles = getAreaStyles;
		vm.openWarningOverlay = openWarningOverlay;
		vm.toggleClearClipboardConfirm = toggleClearClipboardConfirm;

		function openWarningOverlay() {
			localizationService.localizeMany(["bento_layoutWarningTitle", "bento_layoutWarningSubtitle", "bento_cancel", "bento_imOkWithThis"]).then(function(data){

				var title = data[0];
				var subTitle = data[1];
				var cancel = data[2];
				var imOkWithThis = data[3];

				vm.warningOverlay = {
					title: title,
					subtitle: subTitle,
					closeButtonLabel: cancel,
					submitButtonLabel: imOkWithThis,
					show: true,
					submit: function (model) {
						vm.warningOverlay.show = false;
						vm.warningOverlay = null;
						$scope.model.submit(vm.selectedLayout);
					},
					close: function (oldModel) {
						vm.warningOverlay.show = false;
						vm.warningOverlay = null;
					}
				};
			});
		}

		function init() {
			for (var i = 0; i < $scope.model.availableLayouts.length; i++) {
				var availableLayout = $scope.model.availableLayouts[i];
				(function (layout) {
					if ($scope.model.blockLayout !== null && layout.alias === $scope.model.blockLayout.alias) {
						vm.selectedLayout = layout;
					}

					vm.layouts.push(layout);
				})(availableLayout);
			}

			for (var i = 0; i < $scope.model.availableInClipboard.length; i++) {
				var clipboardItem = $scope.model.availableInClipboard[i];
				(function (layout) {
					if ($scope.model.blockLayout !== null && layout.alias === $scope.model.blockLayout.alias) {
						vm.clipSelected = layout;
					}

					vm.clipboard.push(layout);
				})(clipboardItem);
			}
		}

		function getLayoutStyle(layout) {
			var layoutStyle = '';

			//setup the columns configuration
			layoutStyle += 'grid-template-columns: ';
			layoutStyle += layout.columns.join() + ";";

			return layoutStyle;
		}

		function getAreaStyles(area) {
			var style = "grid-column: " + area.column + ";";
			style += " ";
			style += "grid-row: " + area.row + ";";

			return style;
		}

		function submit() {
			if ($scope.model.submit) {
				if ($scope.model.blockLayout.areas && $scope.model.blockLayout.areas.length > vm.selectedLayout.areas.length) {
					vm.openWarningOverlay();
				} else {
					if (vm.selectedLayout !== null) {
						$scope.model.submit(vm.selectedLayout);
					} else {
						$scope.model.submit(vm.selectedClip);
					}
				}
			}
		}

		function toggleClearClipboardConfirm() {
			localizationService.localizeMany(["bento_clearclipboarddescription", "bento_clearclipboard", "general_cancel", "bento_yesclear"]).then(function (data) {
				const overlay = {
					title: data[1],
					content: data[0],
					closeButtonLabel: data[2],
					submitButtonLabel: data[3],
					submitButtonStyle: "danger",
					close: () => {
						overlayService.close();
					},
					submit: () => {
						clipboardService.clearEntriesOfType(clipboardService.TYPES.RAW, ['layout']);
						vm.clipboard = [];
						overlayService.close();
					}
				};

				overlayService.open(overlay);
			});
		}

		function close() {
			if ($scope.model.close) {
				$scope.model.close();
			}
		}

		function layoutSelected(selectedLayout) {
			if (vm.selectedLayout !== null && selectedLayout.alias === vm.selectedLayout.alias) {
				return;
			}

			vm.selectedLayout = selectedLayout;
			vm.submit();
		}

		function clipboardSelected(selectedClip) {
			vm.selectedClip = selectedClip.data;
			vm.submit();
		}

		init();
	}

	angular.module("umbraco").controller("bento.layouts.editor.controller", bentoLayoutsEditorController);
})();