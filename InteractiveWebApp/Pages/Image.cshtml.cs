using ILNumerics.Drawing.Plotting;
using ILNumerics.Drawing;
using ILNumerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Imaging;

namespace InteractiveWebApp.Pages
{
    public class ImageModel : PageModel
    {
        public ActionResult OnGet(int rotX, int rotY, int rotZ)
        {
            Array<float> A = SpecialData.sincf(40, 30, 2);

            var scene = new Scene() {
                new PlotCube(twoDMode: false) {
                    new Surface(A) {
                        Children = {
                            new Colorbar()
                        },
                        UseLighting = true, 
                        Colormap = Colormaps.Jet
                    }
                }
            };
            var gdi = new GDIDriver(800, 600, scene, System.Drawing.Color.White);
            var pc = scene.First<PlotCube>();
            var rotation = Matrix4.Rotation(Vector3.UnitZ, rotZ / 180f) *
                            Matrix4.Rotation(Vector3.UnitY, rotY / 180f) *
                            Matrix4.Rotation(Vector3.UnitX, rotX / 180f);

            // remove ILN branding: scene.Screen.First<Label>().Text = ""; 
            pc.Transform = rotation;
            gdi.Configure();
            gdi.Render();
            MemoryStream ms = new MemoryStream();
            gdi.BackBuffer.Bitmap.Save(ms, ImageFormat.Png);

            return File(ms.GetBuffer(), "image/png");
        }

    }
}
