const { watch, src, dest } = require('gulp');

const sourceFolders = [
	'Bento.Editor/App_Plugins/'
];

const destination = 'Bento.Website/App_Plugins/';

function copy(path, baseFolder) {
	return src(path, { base: baseFolder })
		.pipe(dest(destination));
}

function time() {
	return '[' + new Date().toISOString().slice(11, -5) + ']';
}

exports.default = function () {
	sourceFolders.forEach(function (sourceFolder) {

		let source = sourceFolder + '**/*';

		watch(source, { ignoreInitial: false })
			.on('change', function (path, stats) {
				console.log(time(), path, sourceFolder, 'changed');
				copy(path, sourceFolder);
			})
			.on('add', function (path, stats) {
				console.log(time(), path, sourceFolder, 'added');
				copy(path, sourceFolder);
			});
	});
};