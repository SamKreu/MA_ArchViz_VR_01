using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System;

public class RecordDirectorWindow : EditorWindow
{
  private const string MotionDataSuffix = ".motiondata.asset";
  private const string AvatarMotionDataSuffix = ".avatarmotiondata.asset";

  private static RecordDirectorWindow Director;

  private readonly List<OvrAvatar> avatarList = new List<OvrAvatar>();
  private readonly List<Transform> transformList = new List<Transform>();

  private float fps = 60f;

  private bool foldoutAvatars = true;
  private bool foldoutTransforms = true;

  private GameObject root;

  [MenuItem("Tools/Record Director")]
  public static void ShowWindow()
  {
    Director = GetWindow<RecordDirectorWindow>(true, "Record Director Tool", true);
  }

  void OnGUI()
  {
    if (GUILayout.Button("Prepare Scene"))
      PrepareScene();

    EditorGUILayout.Space();

    fps = EditorGUILayout.FloatField("Motion FPS", fps);
    EditorUtils.DrawAddItem(transformList, "Transform");
    EditorUtils.DrawList(transformList, "Transforms", ref foldoutTransforms);

    EditorGUILayout.Space();

    EditorUtils.DrawAddItem(avatarList, "Avatar");
    EditorUtils.DrawList(avatarList, "Avatars", ref foldoutAvatars);
  }

  private void PrepareScene()
  {
    if (transformList.Count == 0 && avatarList.Count == 0)
      return;

    string path = Path.Combine("Assets", "MotionData");
    Directory.CreateDirectory(path);

    string newFolder = SceneManager.GetActiveScene().name;
    string suffix = "";
    int i = 1;
    while (Directory.Exists(Path.Combine(path, newFolder + suffix)))
      suffix = i++.ToString();

    path = Path.Combine(path, newFolder + suffix);
    Directory.CreateDirectory(path);

    root = GameObject.Find("MotionTool Logic");
    if (root)
      Destroy(root.gameObject);

    root = new GameObject("MotionTool Logic");

    MotionRecorder motionRecorder = null;
    MotionReplayer motionReplayer = null;
    if (transformList.Count > 0)
      PrepareMotionTransfer(transformList.ToArray(), path, out motionRecorder, out motionReplayer).transform.parent = root.transform;

    AvatarRecorder[] avatarRecorders = new AvatarRecorder[avatarList.Count];
    AvatarReplayer[] avatarReplayers = new AvatarReplayer[avatarList.Count];
    for (i = 0; i < avatarList.Count; ++i)
      PrepareAvatarTransfer(avatarList[i], path, out avatarRecorders[i], out avatarReplayers[i]);

    PrepareRecordSyncer(motionRecorder, avatarRecorders).transform.parent = root.transform;
    PrepareReplaySyncer(motionReplayer, avatarReplayers).transform.parent = root.transform;
  }

  private GameObject PrepareMotionTransfer(Transform[] transforms, string dataFolder, out MotionRecorder recorder, out MotionReplayer replayer)
  {
    GameObject motionTransferGO = new GameObject("Motion Transfer");
    recorder = motionTransferGO.AddComponent<MotionRecorder>();
    recorder.enabled = false;
    replayer = motionTransferGO.AddComponent<MotionReplayer>();
    replayer.enabled = false;

    string path = Path.Combine(dataFolder, SceneManager.GetActiveScene().name + MotionDataSuffix);
    MotionData data = EditorUtils.CreateNewAssetAtPath<MotionData>(path);
    recorder.motionData = data;
    replayer.motionData = data;

    Array.Resize(ref recorder.nodes, transforms.Length);
    transforms.CopyTo(recorder.nodes, 0);
    Array.Resize(ref replayer.nodes, transforms.Length);
    transforms.CopyTo(replayer.nodes, 0);

    data.Init(transforms, 0f);
    data.fps = fps;

    return motionTransferGO;
  }

  private void PrepareAvatarTransfer(OvrAvatar avatar, string dataFolder, out AvatarRecorder recorder, out AvatarReplayer replayer)
  {
    recorder = EditorUtils.AddDisabledMonoBehaviour<AvatarRecorder>(avatar);
    replayer = EditorUtils.AddDisabledMonoBehaviour<AvatarReplayer>(avatar);

    string path = Path.Combine(dataFolder, avatar.name + AvatarMotionDataSuffix);
    AvatarMotionData data = EditorUtils.CreateNewAssetAtPath<AvatarMotionData>(path);
    recorder.data = data;
    replayer.data = data;
  }

  private GameObject PrepareRecordSyncer(MotionRecorder motionRecorder, AvatarRecorder[] avatarRecorders)
  {
    GameObject recordSyncerGO = new GameObject("Record Syncer");
    RecordSyncer recordSyncer = recordSyncerGO.AddComponent<RecordSyncer>();
    recordSyncerGO.SetActive(false);

    recordSyncer.motionRecorder = motionRecorder;
    Array.Resize(ref recordSyncer.avatarRecorders, avatarRecorders.Length);
    avatarRecorders.CopyTo(recordSyncer.avatarRecorders, 0);

    return recordSyncerGO;
  }

  private GameObject PrepareReplaySyncer(MotionReplayer motionReplayer, AvatarReplayer[] avatarReplayers)
  {
    GameObject replaySyncerGO = new GameObject("Replay Syncer");
    ReplaySyncer replaySyncer = replaySyncerGO.AddComponent<ReplaySyncer>();
    replaySyncerGO.SetActive(false);

    replaySyncer.motionReplayer = motionReplayer;
    Array.Resize(ref replaySyncer.avatarReplayers, avatarReplayers.Length);
    avatarReplayers.CopyTo(replaySyncer.avatarReplayers, 0);

    return replaySyncerGO;
  }
}
