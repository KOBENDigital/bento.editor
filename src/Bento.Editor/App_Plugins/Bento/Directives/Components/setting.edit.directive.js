(function () {
	'use strict';

	function BentoSettingEditDirective() {

		var directive = {
			restrict: 'E',
			replace: true,
			templateUrl: '/App_Plugins/Bento/Views/Components/bento-setting-edit.html',
			scope: {
				setting: "=",
				settingChanged: "="
			},
			controller: function ($scope, editorService, notificationsService) {
				$scope.openPreValueEditor = openPreValueEditor;
				$scope.remove = remove;
				
				function openPreValueEditor(preValue, index) {
					var preValues = $scope.setting.preValues;

					var options = {
						view: "/App_Plugins/Bento/prevalueeditors/bentoprevalue.edit.html",
						preValue: preValue,
						size: "small",
						submit: function (model) {
							if (model === null) {
								notificationsService.error("No preValue returned");
								return;
							}

							if (typeof index !== "undefined" && index !== null && index >= 0 && index < preValues.length) {
								preValues[index] = model;
							}
							else {
								preValues.push(model);
							}

							editorService.close();
						},
						close: function () {
							editorService.close();
						}
					};

					editorService.open(options);
				}

				function remove(index) {
					$scope.setting.preValues.splice(index, 1);
				}

				function init() {
					if (!$scope.setting.preValues) {
						$scope.setting.preValues = [];
					}

					$scope.sortableOptions = {
						distance: 10,
						tolerance: "pointer",
						scroll: true,
						zIndex: 6000
					};

					if ($scope.settingChanged) {
						$scope.sortableOptions.update = $scope.settingChanged;
					}
				}

				init();
			}
		};

		return directive;
	}

	angular.module('umbraco.directives').directive('bentoSettingEdit', BentoSettingEditDirective);

})();
