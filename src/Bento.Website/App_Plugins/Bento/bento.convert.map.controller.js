(function () {
	'use strict';

	function bentoConvertMapController($scope, localizationService, contentResource) {

		var vm = this;

		vm.close = close;
		vm.submit = submit;
		vm.itemid = $scope.model.itemid;
		vm.documentTypeAlias = $scope.model.documentTypeAlias;
		vm.documentTypeName = $scope.model.documentTypeName;
		vm.map = [];
		vm.oldValues = [];
		vm.options = [];


		function init() {



			//
			contentResource.getById(vm.itemid).then(function (data) {

				for (var v = 0; v < data.variants.length; v++) {
					var variant = data.variants[v];
					for (var t = 0; t < variant.tabs.length; t++) {
						var tab = variant.tabs[t];
						for (var p = 0; p < tab.properties.length; p++) {

							var prop = tab.properties[p];

							vm.oldValues.push({ alias: prop.alias, value: prop.value });
							vm.options.push(prop.alias);

						}
					}
				}
			}).finally(function () {
				// load the new content type
				contentResource.getScaffold(-20, $scope.model.documentTypeAlias).then(function (data) {

					//this is the model node that will be passed back on conversion
					$scope.model.node = data;

					for (var v = 0; v < data.variants.length; v++) {
						var variant = data.variants[v];
						for (var t = 0; t < variant.tabs.length; t++) {
							var tab = variant.tabs[t];
							for (var p = 0; p < tab.properties.length; p++) {

								var prop = tab.properties[p];


								//try match to an old prop with the same alias
								var oldProp = _.findWhere(vm.oldValues, { alias: prop.alias });

								vm.map.push({ new: prop.alias, old: oldProp ? oldProp.alias : undefined });

							}
						}
					}

				});
			});
		}

		function convert() {


			for (var v = 0; v < $scope.model.node.variants.length; v++) {
				var variant = $scope.model.node.variants[v];
				for (var t = 0; t < variant.tabs.length; t++) {
					var tab = variant.tabs[t];


					for (var p = 0; p < tab.properties.length; p++) {
						var prop = tab.properties[p];

						//get the map for the property
						var map = _.findWhere(vm.map, { new: prop.alias });

						//if a map exists
						if (map !== undefined) {

							//try get the old value for it
							var val = _.findWhere(vm.oldValues, { alias: map.old });

							//if the value exists
							if (val !== undefined) {

								//set the value of the new model
								prop.value = val.value;
							}
						}

					}
				}
			}
		}

		function close() {
			if ($scope.model.close) {
				$scope.model.close();
			}
		}


		function openConvertOverlay() {

			localizationService.localizeMany(["bento_mappingwarning", "bento_mappingwarningsubtitle", "bento_cancel", "bento_imOkWithThis"]).then(function (data) {

				var title = data[0];
				var subTitle = data[1];
				var cancel = data[2];
				var imOkWithThis = data[3];
				vm.convertOverlay = {
					title: title,
					subtitle: subTitle,
					closeButtonLabel: cancel,
					submitButtonLabel: imOkWithThis,
					show: true,
					submit: function (model) {
						vm.convertOverlay.show = false;
						vm.convertOverlay = null;
						convert();
						$scope.model.submit($scope.model);
					},
					close: function (oldModel) {
						vm.convertOverlay.show = false;
						vm.convertOverlay = null;
					}
				};
			});
		}


		function submit() {
			if ($scope.model.submit) {
				openConvertOverlay();
			}
		}


		init();
	}

	angular.module('umbraco').controller('bento.convert.map.controller', bentoConvertMapController);
})();
