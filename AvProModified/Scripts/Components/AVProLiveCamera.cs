using UnityEngine;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
// Copyright 2012-2018 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProLiveCamera
{
	[AddComponentMenu("AVPro Live Camera/Live Camera")]
	public class AVProLiveCamera : MonoBehaviour
	{
		// Selected
		protected AVProLiveCameraDevice _device = null;
		protected AVProLiveCameraDeviceMode _mode = null;
		protected int _videoInput = -1;

		// Device selection
		public SelectDeviceBy _deviceSelection = SelectDeviceBy.Default;
		public List<string> _desiredDeviceNames = new List<string>(4);
		public int _desiredDeviceIndex = 0;

		// Mode selection
		public SelectModeBy _modeSelection = SelectModeBy.Default;
		public List<Vector2> _desiredResolutions = new List<Vector2>(2);
		public int _desiredModeIndex = -1;
		public bool _maintainAspectRatio = false;
		public float _desiredFrameRate = 0f;
		public bool _desiredFormatAny = true;
		public AVProLiveCameraPlugin.VideoFrameFormat _desiredFormat = AVProLiveCameraPlugin.VideoFrameFormat.YUV_422_HDYC;

		// Video Input selection
		public SelectDeviceBy _videoInputSelection = SelectDeviceBy.Default;
		public List<AVProLiveCameraPlugin.VideoInput> _desiredVideoInputs = new List<AVProLiveCameraPlugin.VideoInput>(4);
		public int _desiredVideoInputIndex = 0;

		public bool _playOnStart = true;
		public bool _deinterlace = false;
		public bool _flipX;
		public bool _flipY;

		public bool _updateHotSwap = false;
		public bool _updateFrameRates = false;
		public bool _updateSettings = false;
		public string  ScreenshotPath;
#if UNITY_5_4_OR_NEWER || (UNITY_5 && !UNITY_5_0 && !UNITY_5_1)
		private System.IntPtr _renderFunc;
#endif

		public AVProLiveCameraDevice Device
		{
			get { return _device; }
		}

		public Texture OutputTexture
		{
			get { if (_device != null) return _device.OutputTexture; return null; }
		}

		public enum SelectDeviceBy
		{
			Default,
			Name,
			Index,
		}

		public enum SelectModeBy
		{
			Default,
			Resolution,
			Index,
		}

		void Reset()
		{
			_videoInput = -1;
			_mode = null;
			_device = null;
			_flipX = _flipY = false;

			_deviceSelection = SelectDeviceBy.Default;
			_modeSelection = SelectModeBy.Default;
			_videoInputSelection = SelectDeviceBy.Default;
			_desiredDeviceNames = new List<string>(4);
			_desiredResolutions = new List<Vector2>(2);
			_desiredVideoInputs = new List<AVProLiveCameraPlugin.VideoInput>(4);
			_desiredDeviceNames.Add("Logitech BRIO");
			_desiredDeviceNames.Add("Logitech HD Pro Webcam C922");
			_desiredDeviceNames.Add("Logitech HD Pro Webcam C920");
			_desiredDeviceNames.Add("Decklink Video Capture");
			_desiredDeviceNames.Add("Logitech Webcam Pro 9000");
			_desiredResolutions.Add(new Vector2(1920, 1080));
			_desiredResolutions.Add(new Vector2(1280, 720));
			_desiredResolutions.Add(new Vector2(640, 360));
			_desiredResolutions.Add(new Vector2(640, 480));
			_desiredVideoInputs.Add(AVProLiveCameraPlugin.VideoInput.Video_Serial_Digital);
			_desiredVideoInputs.Add(AVProLiveCameraPlugin.VideoInput.Video_SVideo);
			_desiredVideoInputIndex = 0;
			_maintainAspectRatio = false;
			_desiredFrameRate = 0f;
			_desiredFormatAny = true;
			_desiredFormat = AVProLiveCameraPlugin.VideoFrameFormat.YUV_422_HDYC;
			_desiredModeIndex = -1;
			_desiredDeviceIndex = 0;
		}

		public void Start()
		{
			if (null == FindObjectOfType(typeof(AVProLiveCameraManager)))
			{
				throw new System.Exception("You need to add AVProLiveCameraManager component to your scene or change the script execution ordering of AVProLiveCameraManager.");
			}

			SelectDeviceAndMode();

			if (_playOnStart)
			{
				Begin();
			}
		}

		public void Begin()
		{
			SelectDeviceAndMode();

			if (_device != null)
			{
				if (_renderRoutine != null)
				{
					StopCoroutine(_renderRoutine);
					_renderRoutine = null;
				}

				_device.Deinterlace = _deinterlace;
				_device.FlipX = _flipX;
				_device.FlipY = _flipY;
				if (!_device.Start(_mode, _videoInput))
				{
					VitaCaptureU.DeviceFailed = true;
					Debug.LogWarning("[AVPro Live Camera] Device failed to start.");
					_device.Close();
					_device = null;
					_mode = null;
					_videoInput = -1;
				}
				else
				{
					if (_renderRoutine == null)
					{
						_renderRoutine = StartCoroutine(RenderRoutine());
					}
				}
			}
		}

		private void Update()
		{
			if (_device != null)
			{
				if (_flipX != _device.FlipX)
					_device.FlipX = _flipX;
				if (_flipY != _device.FlipY)
					_device.FlipY = _flipY;

				_device.UpdateHotSwap = _updateHotSwap;
				_device.UpdateFrameRates = _updateFrameRates;
				_device.UpdateSettings = _updateSettings;

				_device.Update(false);
			}
		}

		private int _lastFrameCount;
		private bool Render()
		{
			bool result = false;
			if (_device != null)
			{
				// Prevent this function being executed again this frame if the camera frame has been updated this frame already
				if (_lastFrameCount != Time.frameCount)
				{
					if (_device != null)
					{
						if (_device.Render())
						{
							_lastFrameCount = Time.frameCount;
							result = true;
						}
						else
						{
							_lastFrameUpdated = AVProLiveCameraPlugin.GetLastFrameUploaded(_device.DeviceIndex);
						}

						{
							int eventId = AVProLiveCameraPlugin.PluginID | (int)AVProLiveCameraPlugin.PluginEvent.UpdateOneTexture | _device.DeviceIndex;
#if UNITY_5_4_OR_NEWER || (UNITY_5 && !UNITY_5_0 && !UNITY_5_1)
							GL.IssuePluginEvent(_renderFunc, eventId);
#else
							GL.IssuePluginEvent(eventId);
#endif
						}
					}
				}
			}
			return result;
		}

		private YieldInstruction _wait = new WaitForEndOfFrame();
		private Coroutine _renderRoutine;
		private int _lastFrameUpdated;

		private System.Collections.IEnumerator RenderRoutine()
		{
			while (Application.isPlaying)
			{
				yield return null;

				if (this.enabled)
				{
					bool hasUpdatedThisFrame = Render();

					// NOTE: in editor, if the game view isn't visible then WaitForEndOfFrame will never complete
					yield return _wait;

					if (!hasUpdatedThisFrame)
					{
						// Try again to get the frame
						if (!Render())
						{
							//Debug.Log("frame dropped :(");
						}
					}
					else
					{
						// If nothing has updated, send another event
						int lastFrameUpdated = AVProLiveCameraPlugin.GetLastFrameUploaded(_device.DeviceIndex);
						if (_lastFrameUpdated == lastFrameUpdated)
						{
							int eventId = AVProLiveCameraPlugin.PluginID | (int)AVProLiveCameraPlugin.PluginEvent.UpdateOneTexture | _device.DeviceIndex;
#if UNITY_5_4_OR_NEWER || (UNITY_5 && !UNITY_5_0 && !UNITY_5_1)
							GL.IssuePluginEvent(_renderFunc, eventId);
#else
							GL.IssuePluginEvent(eventId);
#endif
						}
					}
				}
			}

			_renderRoutine = null;
		}

		public void OnDestroy()
		{
			if (_renderRoutine != null)
			{
				StopCoroutine(_renderRoutine);
				Debug.Log ("FLush");
				_renderRoutine = null;
			}

			if (_device != null)
				_device.Close();
			_device = null;
		}

		public void SelectDeviceAndMode()
		{
			_device = null;
			_mode = null;
			_videoInput = -1;

			_device = SelectDevice();
			if (_device != null)
			{
				VitaCaptureU.PSVitaConnected=true;
				_mode = SelectMode();
				_videoInput = SelectVideoInputIndex();
			}
			else
			{
				Debug.LogWarning("[AVProLiveCamera] Could not find the device.");
				VitaCaptureU.PSVitaConnected=false;
			}
		}

		private AVProLiveCameraDeviceMode SelectMode()
		{
			AVProLiveCameraDeviceMode result = null;

			switch (_modeSelection)
			{
				default:
				case SelectModeBy.Default:
					result = null;
					break;
				case SelectModeBy.Resolution:
					if (_desiredResolutions.Count > 0)
					{
						result = GetClosestMode(_device, _desiredResolutions, _maintainAspectRatio, _desiredFrameRate, _desiredFormatAny, _desiredFormat);
						if (result == null)
						{
							Debug.LogWarning("[AVProLiveCamera] Could not find desired mode, using default mode.");
						}
					}
					break;
				case SelectModeBy.Index:
					if (_desiredModeIndex >= 0)
					{
						result = _device.GetMode(_desiredModeIndex);
						if (result == null)
						{
							Debug.LogWarning("[AVProLiveCamera] Could not find desired mode, using default mode.");
						}
					}
					break;
			}

			return result;
		}

		private AVProLiveCameraDevice SelectDevice()
		{
			AVProLiveCameraDevice result = null;

			switch (_deviceSelection)
			{
				default:
				case SelectDeviceBy.Default:
					result = AVProLiveCameraManager.Instance.GetDevice(0);
					break;
				case SelectDeviceBy.Name:
					if (_desiredDeviceNames.Count > 0)
					{
						result = GetFirstDevice(_desiredDeviceNames);
					}
					break;
				case SelectDeviceBy.Index:
					if (_desiredDeviceIndex >= 0)
					{
						result = AVProLiveCameraManager.Instance.GetDevice(_desiredDeviceIndex);
					}
					break;
			}

			return result;
		}

		private int SelectVideoInputIndex()
		{
			int result = -1;

			switch (_videoInputSelection)
			{
				default:
				case SelectDeviceBy.Default:
					result = -1;
					break;
				case SelectDeviceBy.Name:
					if (_desiredVideoInputs.Count > 0 && _device.NumVideoInputs > 0)
					{
						foreach (AVProLiveCameraPlugin.VideoInput videoInput in _desiredVideoInputs)
						{
							for (int i = 0; i < _device.NumVideoInputs; i++)
							{
								if (videoInput.ToString().Replace("_", " ") == _device.GetVideoInputName(i))
								{
									result = i;
									break;
								}
							}
							if (result >= 0)
								break;
						}
					}
					break;
				case SelectDeviceBy.Index:
					if (_desiredVideoInputIndex >= 0)
					{
						result = _desiredVideoInputIndex;
					}
					break;
			}

			return result;
		}

		void OnEnable()
		{
			if (_device != null)
			{
				_device.IsActive = true;
			}

#if UNITY_5_4_OR_NEWER || (UNITY_5 && !UNITY_5_0 && !UNITY_5_1)
			if (_renderFunc == System.IntPtr.Zero)
			{
				_renderFunc = AVProLiveCameraPlugin.GetRenderEventFunc();
			}
#endif
		}

		void OnDisable()
		{
			if (_device != null)
			{
				_device.IsActive = false;
			}
		}

		private static AVProLiveCameraDeviceMode GetClosestMode(AVProLiveCameraDevice device, List<Vector2> resolutions, bool maintainApectRatio, float frameRate, bool anyPixelFormat, AVProLiveCameraPlugin.VideoFrameFormat pixelFormat)
		{
			AVProLiveCameraDeviceMode result = null;
			for (int i = 0; i < resolutions.Count; i++)
			{
				result = device.GetClosestMode(Mathf.FloorToInt(resolutions[i].x), Mathf.FloorToInt(resolutions[i].y), maintainApectRatio, frameRate, anyPixelFormat, pixelFormat);
				if (result != null)
					break;
			}
			return result;
		}

		private static AVProLiveCameraDevice GetFirstDevice(List<string> names)
		{
			AVProLiveCameraDevice result = null;
			for (int i = 0; i < names.Count; i++)
			{
				result = AVProLiveCameraManager.Instance.GetDevice(names[i]);
				if (result != null)
					break;
			}
			return result;
		}


	
		public bool SavePNG()
		{
			if (OutputTexture != null && _device != null)
			{
				Texture2D tex = new Texture2D(OutputTexture.width, OutputTexture.height, TextureFormat.ARGB32, false);
				RenderTexture.active = (RenderTexture)OutputTexture;
				tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0, false);
				tex.Apply(false, false);

				byte[] pngBytes = tex.EncodeToJPG(100);
				string date = System.DateTime.Now.ToString();
				date = date.Replace(":","_").Replace(" ","_").Replace ("/","_");
				int randomint = Random.Range(0, 65536);

				//Debug.Log (date);
				System.IO.File.WriteAllBytes("PSVita_"+ date + randomint + ".jpg", pngBytes);

				RenderTexture.active = null;
				Texture2D.Destroy(tex);
				tex = null;
				ScreenshotPath = "PSVita_"+ date + randomint + ".jpg";
				return(true);
			}
			return(false);
		}



	}
}
