using Qt.Core;
using Qt.Gui;

public class TriangleWindow : OpenGLWindow
{
    OpenGLShaderProgram m_program;
    int m_posAttr;
    int m_colAttr;
    int m_matrixUniform;
    int m_frame;

    public override void InitializeGL() {
        InitializeOpenGLFunctions();
        m_program = new OpenGLShaderProgram(this);
        if (!m_program.AddShaderFromSourceCode(OpenGLShader.Vertex, vertexShaderSource)) {
            Console.WriteLine("Vertex Failed!");
        }
        if (!m_program.AddShaderFromSourceCode(OpenGLShader.Fragment, fragmentShaderSource)) {
            Console.WriteLine("Fragment Failed!");
        }
        m_program.Link();
        m_posAttr = m_program.AttributeLocation("posAttr");
        m_colAttr = m_program.AttributeLocation("colAttr");
        m_matrixUniform = m_program.UniformLocation("matrix");
    }

    public override void PaintGL() {
        float retinaScale = DevicePixelRatio();
        glViewport(0, 0, (int)(Width() * retinaScale), (int)(Height() * retinaScale));

        glClear(GL_COLOR_BUFFER_BIT);

        m_program.Bind();

        Matrix4x4 matrix = new Matrix4x4();
        matrix.Perspective(60.0f, 4.0f/3.0f, 0.1f, 100.0f);
        matrix.Translate(0, 0, -2);
        matrix.Rotate(100.0f * m_frame / GetScreen().RefreshRate(), 0, 1, 0);

        m_program.SetUniformValue(m_matrixUniform, matrix);

        float[] vertices = {
            0.0f, 0.707f,
            -0.5f, -0.5f,
            0.5f, -0.5f
        };

        float[] colors = {
            1.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f,
            0.0f, 0.0f, 1.0f
        };

        glVertexAttribPointer(m_posAttr, 2, GL_FLOAT, GL_FALSE, 0, vertices);
        glVertexAttribPointer(m_colAttr, 3, GL_FLOAT, GL_FALSE, 0, colors);

        glEnableVertexAttribArray(0);
        glEnableVertexAttribArray(1);

        glDrawArrays(GL_TRIANGLES, 0, 3);

        glDisableVertexAttribArray(1);
        glDisableVertexAttribArray(0);

        m_program.Release();

        ++m_frame;

        Update();
    }

    public override void KeyPressed(int key) {
        Console.WriteLine("KeyPressed");
    }

    public override void KeyReleased(int key) {
        Console.WriteLine("KeyReleased");
    }

    public override void KeyTyped(char key) {
        Console.WriteLine("KeyTyped");
    }

    public override void MousePressed(int x, int y, int button) {
        Console.WriteLine("MousePressed");
    }

    public override void MouseReleased(int x, int y, int button) {
        Console.WriteLine("MouseReleased");
    }

    public override void MouseMoved(int x, int y, int button) {
        Console.WriteLine("MouseMoved");
    }

    public static void Main(string[] args)
    {
        GuiApplication app = new GuiApplication();

        SurfaceFormat format = new SurfaceFormat();
        format.SetSamples(32);

        TriangleWindow window = new TriangleWindow();
        window.SetFormat(format);
        window.Resize(640, 480);
        window.Show();

        app.Exec();
    }

    string vertexShaderSource =
        "#version 120\n" +
        "attribute highp vec4 posAttr;\n" +
        "attribute lowp vec4 colAttr;\n" +
        "varying lowp vec4 col;\n" +
        "uniform highp mat4 matrix;\n" +
        "void main() {\n" +
        "   col = colAttr;\n" +
        "   gl_Position = matrix * posAttr;\n" +
        "}\n";

    string fragmentShaderSource =
        "#version 120\n" +
        "varying lowp vec4 col;\n" +
        "void main() {\n" +
        "   gl_FragColor = col;\n" +
        "}\n";
}
