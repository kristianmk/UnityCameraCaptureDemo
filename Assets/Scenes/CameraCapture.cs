// Written by K. M. Knausgård 2022-01-24

using System.Collections;
using System.IO;
using UnityEngine;
using System.Collections.Concurrent;
//using System.Threading;
using System.Threading.Tasks;


public class CameraCapture : MonoBehaviour
{
    private bool running = true;
    //private float fixedDeltaTime;
    private int ii = 0;
    private string imageFolderName = "CameraImages";
    private string imagePath;


    public int numberOfIterations { get; private set; } = 20;

    ConcurrentQueue<(int sequenceNumber, byte[] bytes)> imageQueue;
    Task imageWriterTask;

    


    void Start()
    {
        Debug.Log("Starting screen capture script..");

        imagePath = Path.Combine(Application.dataPath, "..", imageFolderName);
        System.IO.Directory.CreateDirectory(imagePath);

        imageQueue = new ConcurrentQueue<(int sequenceNumber, byte[] bytes)>();

        //fixedDeltaTime = Time.fixedDeltaTime;

        StartCoroutine(FFWDSceneCapture());

        System.Action imageWriterAction = () =>
        {

            (int sequenceNumber, byte[] bytes) imageContainer;
            while (running)
            {
                while (imageQueue.TryDequeue(out imageContainer))
                {
                    string path = Path.Combine( imagePath,
                                                "SavedScreen_" + imageContainer.sequenceNumber + ".png");
                    File.WriteAllBytes(path, imageContainer.bytes);
                }
            }
        };

        // Start 1 concurrent consuming action, through a task. Only one to be nice to disks and file system.
        imageWriterTask = Task.Factory.StartNew(imageWriterAction);
    }


    private void Update()
    {
        // Do nothing (see SphereWalker.cs for example of interaction with objects).
    }


    IEnumerator FFWDSceneCapture()
    {
        //Time.timeScale = 100.0f;  // Rudimentary speed-up.
        //Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;

        while (ii++ < 20)
        {   
            Debug.Log("Saving no. " + ii);
            yield return SaveScreenPNG(ii);
        }

        running = false;
        imageWriterTask.Wait();

        if (Application.isBatchMode)
        {
            Debug.Log("Exiting from batchmode (command line option -batchmode)");
            Application.Quit();
        }
    }


    // Based on example:
    // https://docs.unity3d.com/ScriptReference/ImageConversion.EncodeArrayToPNG.html
    IEnumerator SaveScreenPNG(int ii) // IEnumerator
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = ImageConversion.EncodeArrayToPNG(tex.GetRawTextureData(), tex.graphicsFormat, (uint)tex.width, (uint)tex.height);
        Object.Destroy(tex);

        imageQueue.Enqueue((ii, bytes));
    }
}