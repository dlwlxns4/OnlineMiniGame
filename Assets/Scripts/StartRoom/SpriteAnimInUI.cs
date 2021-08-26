using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SpriteAnimInUI : MonoBehaviour
{
    public float _Speed = 1f;
    public bool _Loop = false;
    public int _FrameRate = 30;
    private Image mImage = null;

    [SerializeField] Sprite[] mSprites;
    private float mTimePerFrame = 0f;
    private float mElapsedTime = 0f;
    private int mCurrentFrame = 0;

    // Start is called before the first frame update
    void Start()
    {
        mImage = GetComponent<Image>();
        enabled=false;
        LoadSpriteSheet();
    }


    private void LoadSpriteSheet(){
        if(mSprites != null && mSprites.Length>0){
            mTimePerFrame = 1f / _FrameRate;
            Play();
        }else
            Debug.LogError("Failed to load sprite Sheet");
    }

    void Play(){
        enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        mElapsedTime += Time.deltaTime *_Speed;
        if(mElapsedTime >= mTimePerFrame){
            ++mCurrentFrame;
            mElapsedTime=0f;
            SetSprite();
            if( mCurrentFrame >= mSprites.Length){
                if( _Loop)
                    mCurrentFrame = 0;
                
            }
        }
    }

    void SetSprite(){
        if(mCurrentFrame >= 0 && mCurrentFrame < mSprites.Length)
            mImage.sprite = mSprites[mCurrentFrame];
    }
}
