function bentoSettingFactory() {
	function convertSettingsObjectToArray(defaultSettingsObject, blockSettingsObject) {
		var settings = [];

		for (var prop in defaultSettingsObject) {
			if (defaultSettingsObject.hasOwnProperty(prop)) {
				var bs = blockSettingsObject ? blockSettingsObject[prop] : null;
				var ds = defaultSettingsObject[prop];

				var setting = buildSetting(prop, bs, ds);
				settings.push(setting);
			}
		}

		return settings;
	}

	function buildSetting(settingsProperty, blockSetting, defaultSetting) {
		var setting = {
			alias: settingsProperty
		};

		var blockSettingMissing = typeof blockSetting === "undefined" || blockSetting === null;
		var blockSettingValueMissing = typeof blockSetting === "undefined" ||
			blockSetting === null ||
			typeof blockSetting.value === "undefined" ||
			blockSetting.value === null;

		for (var defaultSettingProperty in defaultSetting) {
			if (defaultSetting.hasOwnProperty(defaultSettingProperty)) {
				setting[defaultSettingProperty] = defaultSetting[defaultSettingProperty];

				if (!blockSettingMissing &&
					defaultSettingProperty !== "preValues" &&
					typeof blockSetting[defaultSettingProperty] !== "undefined" &&
					blockSetting[defaultSettingProperty] !== null) {
					setting[defaultSettingProperty] = blockSetting[defaultSettingProperty];
				}
			}
		}

		if (setting.type === "checklist") {
			if (typeof setting.preValues !== "undefined" && setting.preValues !== null) {
				var valueArray;

				if (!blockSettingMissing && !blockSettingValueMissing) {
					valueArray = blockSetting.value;
				} else {
					valueArray = defaultSetting.value;
				}

				setSelectedPreValues(setting, valueArray);
			}

			try {
				//drop the value prop as we don't need it on this type of setting
				delete setting.value;
			} catch (e) {
				//do nothing as this should never fail but we would rather it threw nothing if it did
			}
		} else {
			if (typeof setting.value === "undefined") {
				if (!blockSettingMissing && !blockSettingValueMissing) {
					setting.value = blockSetting.value;
				} else {
					//default the value prop as we need it on these type of settings
					setting.value = setting.type === "boolean" ? false : null;
				}
			}
		}

		return setting;
	}

	function setSelectedPreValues(setting, valueArray) {
		if (typeof valueArray !== "undefined" && valueArray !== null && valueArray.length > 0) {
			for (var i = 0; i < valueArray.length; i++) {
				var value = valueArray[i];
				for (var j = 0; j < setting.preValues.length; j++) {
					var preValue = setting.preValues[j];
					if (preValue.selected) {
						continue;
					} else if (value === preValue.value) {
						preValue.selected = true;
					} else {
						preValue.selected = false;
					}
				}
			}
		}
	}

	function buildSettingsForSave(settings, errors, valueNotRequired) {
		var settingsForSave = {};

		for (var i = 0; i < settings.length; i++) {
			var blockSetting = settings[i];

			if (!valueNotRequired) {
				if (typeof blockSetting.value === "undefined" || blockSetting.value === null) {
					if (blockSetting.required &&
					(blockSetting.type === "string" ||
						blockSetting.type === "text" ||
						blockSetting.type === "radio" ||
						(blockSetting.type === "dropdown" && !blockSetting.multiple))) {
						errors.push(blockSetting.label + " is required");
						continue;
					}
				} else {
					if ((blockSetting.type === "string" || blockSetting.type === "text")) {
						blockSetting.value = blockSetting.value.trimLeft();

						if (blockSetting.required && blockSetting.value.length <= 0) {
							errors.push(blockSetting.label + " is required");
							continue;
						}

						if (blockSetting.value.length > 0 &&
							typeof blockSetting.validationPattern !== "undefined" &&
							blockSetting.validationPattern !== null) {
							var regex = new RegExp(blockSetting.validationPattern);

							if (!regex.test(blockSetting.value)) {
								errors.push(blockSetting.label + " has an invalid value");
								continue;
							}
						}
					}
				}
			}

			var newSetting = buildSettingForSave(blockSetting, errors, valueNotRequired);

			if (newSetting !== null) {
				settingsForSave[blockSetting.alias] = newSetting;
			}
		}

		return settingsForSave;
	}

	function buildSettingForSave(setting, errors, valueNotRequired) {
		var newSetting = iterationCopy(setting);
		
		if (!valueNotRequired) {
			if (setting.type === "checklist") {
				newSetting.value = [];

				for (var j = 0; j < setting.preValues.length; j++) {
					var preValue = setting.preValues[j];

					if (preValue.selected) {
						(function(value) {
							newSetting.value.push(value);
						})(preValue.value);
					}
				}

				if (newSetting.required && newSetting.value.length <= 0) {
					errors.push(newSetting.label + " is required");
					return null;
				}
			}

			if (setting.type === "dropdown") {
				if (setting.multiple) {
					var newValue = [];

					for (var valueProp in setting.value) {
						if (setting.value.hasOwnProperty(valueProp)) {
							newValue.push(setting.value[valueProp]);
						}
					}

					newSetting.value = newValue;

					if (newSetting.required && newSetting.value.length <= 0) {
						errors.push(newSetting.label + " is required");
						return null;
					}
				} else {
					if (setting.required && (setting.value === null || setting.value.length <= 0)) {
						errors.push(setting.label + " is required");
						return null;
					}
				}
			}

			cleanUpSettingForSave(newSetting);
		} else {
			if (setting.type === "radio" || setting.type === "checklist" || setting.type === "dropdown") {
				var newPreValues = [];

				for (var preValueProp in setting.preValues) {
					if (setting.preValues.hasOwnProperty(preValueProp)) {
						newPreValues.push(setting.preValues[preValueProp]);
					}
				}

				newSetting.preValues = newPreValues;
			}

			try {
				delete newSetting["$$hashKey"];
			} catch (e) {
				// we would rather it threw nothing if it fails
			}
		}

		return newSetting;
	}

	function cleanUpSettingForSave(newSetting) {
		try {
			delete newSetting.alias;
		} catch (e) {
			//do nothing as this should never fail but we would rather it threw nothing if it did
		}

		try {
			delete newSetting.preValues;
		} catch (e) {
			// we would rather it threw nothing if it fails
		}

		try {
			delete newSetting.validationPattern;
		} catch (e) {
			// we would rather it threw nothing if it fails
		}

		try {
			delete newSetting.multiple;
		} catch (e) {
			// we would rather it threw nothing if it fails
		}

		try {
			delete newSetting.required;
		} catch (e) {
			// we would rather it threw nothing if it fails
		}

		try {
			delete newSetting["$$hashKey"];
		} catch (e) {
			// we would rather it threw nothing if it fails
		}
	}

	function isObject(obj) {
		var type = typeof obj;
		return type === "function" || type === "object" && !!obj;
	};

	function iterationCopy(src) {
		let target = {};
		for (let prop in src) {
			if (src.hasOwnProperty(prop)) {
				// if the value is a nested object, recursively copy all it's properties
				if (isObject(src[prop])) {
					target[prop] = iterationCopy(src[prop]);
				} else {
					target[prop] = src[prop];
				}
			}
		}
		return target;
	}

	return {
		buildSetting: buildSetting,
		buildSettingsForSave: buildSettingsForSave,
		setSelectedPreValues: setSelectedPreValues,
		convertSettingsObjectToArray: convertSettingsObjectToArray
	};
}

angular.module('umbraco.resources').factory('bentoSettingFactory', bentoSettingFactory);