using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bento.Core.DataEditors
{
    public static class DataEditorConstants
    {
        public static string Credits { get {

				Assembly assembly = Assembly.GetExecutingAssembly();
				System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
				string version = fvi.FileVersion;

				return @"<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 386.41 94.89"" style=""max-width:250px; margin-bottom:1em;"">
			<defs>
				<style>
					.a {
						fill: #f5c1bc;
					}

					.b {
						fill: #1a264f;
					}
				</style>
			</defs>
			<path class=""a"" d=""M111.6,392.61,56.32,411.22a10.52,10.52,0,0,0-6.6,13.3l18.62,55.27a10.5,10.5,0,0,0,13.3,6.6l55.27-18.61a10.51,10.51,0,0,0,6.6-13.3L124.9,399.2A10.51,10.51,0,0,0,111.6,392.61Zm-3.16,52.76-13.27,4.46-4.46-13.26,10-3.35a3.5,3.5,0,0,1,4.43,2.2l3.35,9.95Zm-5.82,26.57-5.21-15.47,35.37-11.92,4.1,12.16a3.52,3.52,0,0,1-2.2,4.44ZM66.41,452.13l19.9-6.7L96,474.18,79.4,479.76a3.5,3.5,0,0,1-4.43-2.2l-8.56-25.43Zm8.73-39.86,8.93,26.53-19.9,6.7-7.81-23.22a3.5,3.5,0,0,1,2.2-4.43ZM81.77,410l32.06-10.79a3.51,3.51,0,0,1,4.44,2.2l12.28,36.48-15.48,5.21-3.35-9.95a10.52,10.52,0,0,0-13.3-6.6l-10,3.35L81.77,410Z"" transform=""translate(-49.17 -392.06)"" />
			<path class=""b"" d=""M197.06,473.45a49.74,49.74,0,0,1-12.48-1.52A36.89,36.89,0,0,1,174,467.37V413l-11-3.79,4-11.72,20.62,7.05V423a21.43,21.43,0,0,1,11.5-3.15q11.94,0,18.07,6.73t6.13,19.86q0,13.33-6.67,20.18T197.06,473.45Zm-.11-11q6.08,0,9.22-4.07t3.15-12q0-7.6-2.6-11.51A8.68,8.68,0,0,0,199,431q-7.26,0-11.39,3.47v26.59A27,27,0,0,0,197,462.49Z"" transform=""translate(-49.17 -392.06)"" />
			<path class=""b"" d=""M275.53,465.2a35.17,35.17,0,0,1-10.37,6.24,30,30,0,0,1-10.58,2A30.41,30.41,0,0,1,239.83,470a24.21,24.21,0,0,1-9.72-9.44,27.32,27.32,0,0,1-3.42-13.73A29.7,29.7,0,0,1,230,432.64a23.62,23.62,0,0,1,9-9.44,26,26,0,0,1,13.34-3.36A28.38,28.38,0,0,1,266,423a22.54,22.54,0,0,1,9,8.73,26.14,26.14,0,0,1,3.2,13.19c0,.94-.05,2-.16,3.25a31.51,31.51,0,0,1-.49,3.47H239.83a11.56,11.56,0,0,0,2.49,6.19,12.28,12.28,0,0,0,5.26,3.69,19.82,19.82,0,0,0,7,1.19,25.64,25.64,0,0,0,8.79-1.52,27.55,27.55,0,0,0,7.71-4.23Zm-11-22.68a11.32,11.32,0,0,0-3.15-8.3c-2.1-2.14-5-3.2-8.68-3.2a12.79,12.79,0,0,0-8.68,3.2,11.12,11.12,0,0,0-3.91,8.3Z"" transform=""translate(-49.17 -392.06)"" />
			<path class=""b"" d=""M280.83,471.28v-11h9.23V435.68l-10.42-3.58L283.22,421l15.3,5.21a46,46,0,0,1,8.68-4.66,30,30,0,0,1,11.07-1.74q7.16,0,11.29,2.82a15.62,15.62,0,0,1,5.91,7.6,29.82,29.82,0,0,1,1.79,10.52v19.54h9v11H319.14v-11h4.45V442.41a13.86,13.86,0,0,0-1.85-7.49Q319.9,432,314.8,432a17.34,17.34,0,0,0-6.24,1.14,19.32,19.32,0,0,0-4.83,2.55v24.64h4.34v11Z"" transform=""translate(-49.17 -392.06)"" />
			<path class=""b"" d=""M370.92,473.45a20.5,20.5,0,0,1-9.87-2.23,14.52,14.52,0,0,1-6.3-6.73,26,26,0,0,1-2.17-11.34V433h-9V422h9v-8.79L365.82,409v13h13v11h-13v17.15c0,3.9.6,6.89,1.79,8.95a5.81,5.81,0,0,0,5.37,3.09A15.75,15.75,0,0,0,380,460l2.6,10.2a40,40,0,0,1-5.64,2.28A19.82,19.82,0,0,1,370.92,473.45Z"" transform=""translate(-49.17 -392.06)"" />
			<path class=""b"" d=""M409.32,419.84a29.58,29.58,0,0,1,14.33,3.2,21.07,21.07,0,0,1,8.89,9.22,31.83,31.83,0,0,1,3,14.49q0,12.58-6.83,19.64t-19.43,7.06a29.08,29.08,0,0,1-14.05-3.15,20.58,20.58,0,0,1-8.79-9.17,36.45,36.45,0,0,1,0-28.87,20.85,20.85,0,0,1,8.79-9.22A28.75,28.75,0,0,1,409.32,419.84Zm.11,11.18q-6.18,0-9.33,4T397,446.75q0,7.71,3.15,11.72t9.33,4q6.3,0,9.5-4t3.2-11.72q0-7.69-3.2-11.72T409.43,431Z"" transform=""translate(-49.17 -392.06)"" />
		</svg>
		<div style=""position:absolute; top:0; right:0;"" class=""version"">v" + version + @"</div>
		<div style=""background: #f6f4f4; color:#1a264f; padding:2em; border-radius:1em;"">
			<p>Created by <a href=""https://koben.com.au"" target=""_blank""><strong>Koben digital</strong></a> and others who have contributed countless hours to create a flexible visual editor experience for Umbraco.</p>
			<p>We choose to provide this to the community free of charge under the MIT license, however if you are using Bento commercially, and it brings joy to your developer life, please consider the effort that it takes to build complex packages like this and give a donation to help keep the project rolling.</p>

			<a class=""btn"" href=""https://github.com/KOBENDigital/bento.editor"">Documentation</a> <a class=""btn"" href=""https://github.com/KOBENDigital/bento.editor/issues"">Report an issue</a> <a class=""btn"" href=""https://github.com/KOBENDigital/bento.editor"">Contribute</a> <a class=""btn btn-primary"" href=""https://github.com/KOBENDigital/bento.editor"">Donate</a>
		</div>";

			} }
    }
}
