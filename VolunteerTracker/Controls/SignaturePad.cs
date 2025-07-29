using System.Drawing.Drawing2D;

namespace VolunteerTracker.Controls
{
    public class SignaturePad : Control
    {
        private List<Point> _points = new();
        private bool _isDrawing = false;
        private Pen _drawingPen;
        private Bitmap? _signatureBitmap;
        private Graphics? _signatureGraphics;

        public SignaturePad()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            _drawingPen = new Pen(Color.White, 2);
            _drawingPen.StartCap = LineCap.Round;
            _drawingPen.EndCap = LineCap.Round;
            _drawingPen.LineJoin = LineJoin.Round;
            
            BackColor = Color.FromArgb(30, 30, 30);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (_signatureBitmap != null)
            {
                e.Graphics.DrawImage(_signatureBitmap, 0, 0);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDrawing = true;
                _points.Clear();
                _points.Add(e.Location);
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_isDrawing && e.Button == MouseButtons.Left)
            {
                _points.Add(e.Location);
                DrawLine();
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDrawing = false;
            }
        }

        private void DrawLine()
        {
            if (_signatureGraphics == null || _points.Count < 2) return;

            var lastPoint = _points[_points.Count - 2];
            var currentPoint = _points[_points.Count - 1];
            _signatureGraphics.DrawLine(_drawingPen, lastPoint, currentPoint);
        }

        public void Clear()
        {
            _points.Clear();
            _signatureBitmap?.Dispose();
            _signatureBitmap = new Bitmap(Width, Height);
            _signatureGraphics?.Dispose();
            _signatureGraphics = Graphics.FromImage(_signatureBitmap);
            _signatureGraphics.Clear(Color.FromArgb(30, 30, 30));
            Invalidate();
        }

        public byte[]? GetSignatureImage()
        {
            if (_points.Count == 0) return null;

            try
            {
                using var ms = new MemoryStream();
                _signatureBitmap?.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
            catch
            {
                return null;
            }
        }

        public bool HasSignature => _points.Count > 0;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0 && Height > 0)
            {
                _signatureBitmap?.Dispose();
                _signatureBitmap = new Bitmap(Width, Height);
                _signatureGraphics?.Dispose();
                _signatureGraphics = Graphics.FromImage(_signatureBitmap);
                _signatureGraphics.Clear(Color.FromArgb(30, 30, 30));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _drawingPen?.Dispose();
                _signatureBitmap?.Dispose();
                _signatureGraphics?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
} 