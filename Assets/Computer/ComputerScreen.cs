using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ComputerMemory))]
public class ComputerScreen : MonoBehaviour {

    public RawImage renderImage;

    const uint paletteStart = 0x7FFD00;
    const uint paletteEnd = 0x7FFFFF;
    const uint memStart = 0x780000;
    static uint memEnd = 0;

    byte[] DefaultPalette256 = new byte[768]
    {
        0, 0, 0,
        128, 0, 0,
        0, 128, 0,
        128, 128, 0,
        0, 0, 128,
        128, 0, 128,
        0, 128, 128,
        192, 192, 192,
        128, 128, 128,
        255, 0, 0,
        0, 255, 0,
        255, 255, 0,
        0, 0, 255,
        255, 0, 255,
        0, 255, 255,
        255, 255, 255,
        0, 0, 0,
        0, 0, 95,
        0, 0, 135,
        0, 0, 175,
        0, 0, 215,
        0, 0, 255,
        0, 95, 0,
        0, 95, 95,
        0, 95, 135,
        0, 95, 175,
        0, 95, 215,
        0, 95, 255,
        0, 135, 0,
        0, 135, 95,
        0, 135, 135,
        0, 135, 175,
        0, 135, 215,
        0, 135, 255,
        0, 175, 0,
        0, 175, 95,
        0, 175, 135,
        0, 175, 175,
        0, 175, 215,
        0, 175, 255,
        0, 215, 0,
        0, 215, 95,
        0, 215, 135,
        0, 215, 175,
        0, 215, 215,
        0, 215, 255,
        0, 255, 0,
        0, 255, 95,
        0, 255, 135,
        0, 255, 175,
        0, 255, 215,
        0, 255, 255,
        95, 0, 0,
        95, 0, 95,
        95, 0, 135,
        95, 0, 175,
        95, 0, 215,
        95, 0, 255,
        95, 95, 0,
        95, 95, 95,
        95, 95, 135,
        95, 95, 175,
        95, 95, 215,
        95, 95, 255,
        95, 135, 0,
        95, 135, 95,
        95, 135, 135,
        95, 135, 175,
        95, 135, 215,
        95, 135, 255,
        95, 175, 0,
        95, 175, 95,
        95, 175, 135,
        95, 175, 175,
        95, 175, 215,
        95, 175, 255,
        95, 215, 0,
        95, 215, 95,
        95, 215, 135,
        95, 215, 175,
        95, 215, 215,
        95, 215, 255,
        95, 255, 0,
        95, 255, 95,
        95, 255, 135,
        95, 255, 175,
        95, 255, 215,
        95, 255, 255,
        135, 0, 0,
        135, 0, 95,
        135, 0, 135,
        135, 0, 175,
        135, 0, 215,
        135, 0, 255,
        135, 95, 0,
        135, 95, 95,
        135, 95, 135,
        135, 95, 175,
        135, 95, 215,
        135, 95, 255,
        135, 135, 0,
        135, 135, 95,
        135, 135, 135,
        135, 135, 175,
        135, 135, 215,
        135, 135, 255,
        135, 175, 0,
        135, 175, 95,
        135, 175, 135,
        135, 175, 175,
        135, 175, 215,
        135, 175, 255,
        135, 215, 0,
        135, 215, 95,
        135, 215, 135,
        135, 215, 175,
        135, 215, 215,
        135, 215, 255,
        135, 255, 0,
        135, 255, 95,
        135, 255, 135,
        135, 255, 175,
        135, 255, 215,
        135, 255, 255,
        175, 0, 0,
        175, 0, 95,
        175, 0, 135,
        175, 0, 175,
        175, 0, 215,
        175, 0, 255,
        175, 95, 0,
        175, 95, 95,
        175, 95, 135,
        175, 95, 175,
        175, 95, 215,
        175, 95, 255,
        175, 135, 0,
        175, 135, 95,
        175, 135, 135,
        175, 135, 175,
        175, 135, 215,
        175, 135, 255,
        175, 175, 0,
        175, 175, 95,
        175, 175, 135,
        175, 175, 175,
        175, 175, 215,
        175, 175, 255,
        175, 215, 0,
        175, 215, 95,
        175, 215, 135,
        175, 215, 175,
        175, 215, 215,
        175, 215, 255,
        175, 255, 0,
        175, 255, 95,
        175, 255, 135,
        175, 255, 175,
        175, 255, 215,
        175, 255, 255,
        215, 0, 0,
        215, 0, 95,
        215, 0, 135,
        215, 0, 175,
        215, 0, 215,
        215, 0, 255,
        215, 95, 0,
        215, 95, 95,
        215, 95, 135,
        215, 95, 175,
        215, 95, 215,
        215, 95, 255,
        215, 135, 0,
        215, 135, 95,
        215, 135, 135,
        215, 135, 175,
        215, 135, 215,
        215, 135, 255,
        215, 175, 0,
        215, 175, 95,
        215, 175, 135,
        215, 175, 175,
        215, 175, 215,
        215, 175, 255,
        215, 215, 0,
        215, 215, 95,
        215, 215, 135,
        215, 215, 175,
        215, 215, 215,
        215, 215, 255,
        215, 255, 0,
        215, 255, 95,
        215, 255, 135,
        215, 255, 175,
        215, 255, 215,
        215, 255, 255,
        255, 0, 0,
        255, 0, 95,
        255, 0, 135,
        255, 0, 175,
        255, 0, 215,
        255, 0, 255,
        255, 95, 0,
        255, 95, 95,
        255, 95, 135,
        255, 95, 175,
        255, 95, 215,
        255, 95, 255,
        255, 135, 0,
        255, 135, 95,
        255, 135, 135,
        255, 135, 175,
        255, 135, 215,
        255, 135, 255,
        255, 175, 0,
        255, 175, 95,
        255, 175, 135,
        255, 175, 175,
        255, 175, 215,
        255, 175, 255,
        255, 215, 0,
        255, 215, 95,
        255, 215, 135,
        255, 215, 175,
        255, 215, 215,
        255, 215, 255,
        255, 255, 0,
        255, 255, 95,
        255, 255, 135,
        255, 255, 175,
        255, 255, 215,
        255, 255, 255,
        8, 8, 8,
        18, 18, 18,
        28, 28, 28,
        38, 38, 38,
        48, 48, 48,
        58, 58, 58,
        68, 68, 68,
        78, 78, 78,
        88, 88, 88,
        98, 98, 98,
        108, 108, 108,
        118, 118, 118,
        128, 128, 128,
        138, 138, 138,
        148, 148, 148,
        158, 158, 158,
        168, 168, 168,
        178, 178, 178,
        188, 188, 188,
        198, 198, 198,
        208, 208, 208,
        218, 218, 218,
        228, 228, 228,
        238, 238, 238,
    };

    class GraphicsMode
    {
        public Texture2D texture;
        public ComputerMemory.MemoryRange range;
        public int pixelsPerByte;
        public Color[] pixels;
        public GraphicsMode(int width, int height, int pixelsPerByte)
        {
            this.pixelsPerByte = pixelsPerByte;
            texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            pixels = new Color[width * height];
            uint size = (uint)(width * height / pixelsPerByte);
            range = new ComputerMemory.MemoryRange(memStart, memStart + size);
            if (memStart + size > memEnd)
            {
                memEnd = memStart + size;
            }
        }
        public float GetTextureRatio()
        {
            return (float)texture.width / texture.height;
        }
    }

    GraphicsMode[] modes;
    Color[] palette = new Color[256];
    ComputerMemory mem;
    int currentMode = 0;
    bool needRedraw = false;
    bool needPaletteChange = false;

    void Start () {
        mem = GetComponent<ComputerMemory>();
        modes = new GraphicsMode[3];
        modes[0] = new GraphicsMode(800, 600, 2);
        modes[1] = new GraphicsMode(800, 600, 1);
        modes[2] = new GraphicsMode(1024, 768, 2);
        ComputerMemory.MemoryRange screenRange = new ComputerMemory.MemoryRange(memStart, memEnd);
        mem.Subscribe("Screen", screenRange, OnScreenMemoryChange);
        ComputerMemory.MemoryRange paletteRange = new ComputerMemory.MemoryRange(paletteStart, paletteEnd);
        mem.Subscribe("ScreenPalette", paletteRange, OnPaletteChange);

        SwitchMode(2);
        WriteTestScreen();
        SetDefaultPalette();
    }

    void SetDefaultPalette()
    {
        uint addr = paletteStart;
        for (int i = 0; i < 768; i++)
        {
            mem.Write8(addr++, DefaultPalette256[i]);
        }
    }

    void SwitchMode(int mode)
    {
        currentMode = mode;
        renderImage.texture = modes[currentMode].texture;
        needRedraw = true;
    }

    void WriteTestScreen()
    {
        byte c = 0;
        GraphicsMode mode = modes[currentMode];
        for (int i = 0; i < mode.texture.width * mode.texture.height / mode.pixelsPerByte; i++)
        {
            ComputerMemory.memory[memStart + i] = c;
            c++;
        }
    }

    public void OnScreenMemoryChange(ComputerMemory.MemoryRange range)
    {
        needRedraw = true;
    }

    public void OnPaletteChange(ComputerMemory.MemoryRange range)
    {
        needPaletteChange = true;
    }
	
	void Update ()
    {
        SetRenderImageSize();
        if (needPaletteChange)
        {
            PaletteChange();
        }
        if (needRedraw)
        {
            Redraw();
        }
    }

    void PaletteChange()
    {
        Debug.Log("Reading new palette");
        uint addr = paletteStart;
        for (int i = 0; i < 256; i++)
        {
            float r = (float)ComputerMemory.memory[addr++] / 255;
            float g = (float)ComputerMemory.memory[addr++] / 255;
            float b = (float)ComputerMemory.memory[addr++] / 255;
            palette[i] = new Color(r, g, b);
        }
        needPaletteChange = false;
        needRedraw = true;
    }

    void Redraw()
    {
        Debug.Log("Redrawing");

        byte data;
        uint addr;
        int colorIndex;
        Color c;
        GraphicsMode mode = modes[currentMode];

        int pixelIndex = 0;
        Debug.Log(string.Format("{0:X8} - {1:X8}", mode.range.start, mode.range.end));
        Debug.Log(string.Format("Pixels length: {0}, Pixels per byte: {1}", mode.pixels.Length, mode.pixelsPerByte));

        switch (mode.pixelsPerByte)
        {
            case 1:
                for (addr = mode.range.start; addr < mode.range.end; addr++)
                {
                    data = ComputerMemory.memory[addr];
                    colorIndex = data;
                    c = palette[colorIndex];
                    mode.pixels[pixelIndex++] = c;
                }
                break;
            case 2:
                for (addr = mode.range.start; addr < mode.range.end; addr++)
                {
                    data = ComputerMemory.memory[addr];
                    colorIndex = (data & 0xF0) >> 4;
                    c = palette[colorIndex];
                    mode.pixels[pixelIndex++] = c;

                    colorIndex = data & 0x0F;
                    c = palette[colorIndex];
                    mode.pixels[pixelIndex++] = c;
                }
                break;
        }
        mode.texture.SetPixels(mode.pixels);
        mode.texture.Apply();
        needRedraw = false;
    }

    private void SetRenderImageSize()
    {
        float screenRatio, textureRatio, height, width;
        screenRatio = (float)Screen.width / Screen.height;
        Texture2D currentTexture = modes[currentMode].texture;
        textureRatio = (float)currentTexture.width / currentTexture.height;
        if (screenRatio >= textureRatio)
        {
            height = Screen.height;
            width = height * textureRatio;
        }
        else
        {
            width = Screen.width;
            height = width / textureRatio;
        }
        renderImage.rectTransform.sizeDelta = new Vector2(width, height);
    }
}
