const { watch, src, dest } = require('gulp');
const sass = require('gulp-sass')(require('sass'));
const sourcemaps = require('gulp-sourcemaps');

const sourceFolders = [
	'Bento.Editor/App_Plugins/'
];

const destination = 'Bento.Website/App_Plugins/';
const sassSourceFolder = 'Bento.Editor/App_Plugins/Bento/Css/';

function buildStyles() {
  return src(`${sassSourceFolder}/bento.scss`)
	.pipe(sourcemaps.init())
    .pipe(sass().on('error', sass.logError))
		.pipe(sourcemaps.write())
    .pipe(dest(`${sourceFolders}/Bento/Css`));
};


function copy(path, baseFolder) {
	return src(path, { base: baseFolder })
		.pipe(dest(destination));
}

function time() {
	return '[' + new Date().toISOString().slice(11, -5) + ']';
}

exports.default = function () {
console.log('Watching for changes in css...' + `${sassSourceFolder}**/*.scss`);
	watch(`${sassSourceFolder}**/*.scss`, { ignoreInitial: false })
	.on('change', function (path, stats) {
		console.log(time(), path, sassSourceFolder, 'changed');
		buildStyles();
	})
	.on('add', function (path, stats) {
		console.log(time(), path, sassSourceFolder, 'added');
		buildStyles();
	});

	console.log('Watching for changes in source folders...');
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