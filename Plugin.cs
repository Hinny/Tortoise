﻿using System;
using System.Collections;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using GorillaLocomotion;
using GorillaNetworking;
using Grate.Extensions;
using Grate.Gestures;
using Grate.GUI;
using Grate.Modules;
using Grate.Networking;
using Grate.Tools;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Grate
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]

    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        public static bool initialized, WaWa_graze_dot_cc;
        bool pluginEnabled = false;
        public static AssetBundle assetBundle;
        public static AssetBundle grateBundle;
        public static MenuController menuController;
        public static GameObject monkeMenuPrefab;
        public static ConfigFile configFile;
        public static bool IsSteam { get; protected set; }
        public static bool DebugMode { get; protected set; } = false;
        GestureTracker gt;
        NetworkPropertyHandler nph;


        public void Setup()
        {
            if (menuController || !pluginEnabled || !WaWa_graze_dot_cc) return;
            Logging.Debug("Menu:", menuController, "Plugin Enabled:", pluginEnabled, "InRoom:", WaWa_graze_dot_cc);
            try
            {
                gt = this.gameObject.GetOrAddComponent<GestureTracker>();
                nph = this.gameObject.GetOrAddComponent<NetworkPropertyHandler>();
                menuController = Instantiate(monkeMenuPrefab).AddComponent<MenuController>();
            }
            catch (Exception e)
            {
                Logging.Exception(e);
            }
        }

        public void Cleanup()
        {
            try
            {
                Logging.Debug("Cleaning up");
                menuController?.gameObject?.Obliterate();
                gt?.Obliterate();
                nph?.Obliterate();
            }
            catch (Exception e)
            {
                Logging.Exception(e);
            }
        }

        void Awake()
        {
            try
            {
                Instance = this;
                Logging.Init();
                CI.Init();
                configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "Grate.cfg"), true);
                Logging.Debug("Found", GrateModule.GetGrateModuleTypes().Count, "modules");
                foreach (Type moduleType in GrateModule.GetGrateModuleTypes())
                {
                    MethodInfo bindConfigs = moduleType.GetMethod("BindConfigEntries");
                    if (bindConfigs is null) continue;
                    bindConfigs.Invoke(null, null);
                }
                MenuController.BindConfigEntries();
            }
            catch (Exception e) { Logging.Exception(e); }
        }

        void Start()
        {
            try
            {
                GorillaTagger.OnPlayerSpawned(OnGameInitialized);
                assetBundle = AssetUtils.LoadAssetBundle("Grate/Resources/barkbundle");
                grateBundle = AssetUtils.LoadAssetBundle("Grate/Resources/gratebundle");
                monkeMenuPrefab = assetBundle.LoadAsset<GameObject>("Bark Menu");
            }
            catch (Exception e)
            {
                Logging.Exception(e);
            }
        }

        public static Text debugText;
        void CreateDebugGUI()
        {
            try
            {
                if (Player.Instance)
                {
                    var canvas = Player.Instance.headCollider.transform.GetComponentInChildren<Canvas>();
                    if (!canvas)
                    {
                        canvas = new GameObject("~~~Grate Debug Canvas").AddComponent<Canvas>();
                        canvas.renderMode = RenderMode.WorldSpace;
                        canvas.transform.SetParent(Player.Instance.headCollider.transform);
                        canvas.transform.localPosition = Vector3.forward * .35f;
                        canvas.transform.localRotation = Quaternion.identity;
                        canvas.transform.localScale = Vector3.one;
                        canvas.gameObject.AddComponent<CanvasScaler>();
                        canvas.gameObject.AddComponent<GraphicRaycaster>();
                        canvas.GetComponent<RectTransform>().localScale = Vector3.one * .035f;
                        var text = new GameObject("~~~Text").AddComponent<Text>();
                        text.transform.SetParent(canvas.transform);
                        text.transform.localPosition = Vector3.zero;
                        text.transform.localRotation = Quaternion.identity;
                        text.transform.localScale = Vector3.one;
                        text.color = Color.green;
                        //text.text = "Hello World";
                        text.fontSize = 24;
                        text.font = Font.CreateDynamicFontFromOSFont("Arial", 24);
                        text.alignment = TextAnchor.MiddleCenter;
                        text.horizontalOverflow = HorizontalWrapMode.Overflow;
                        text.verticalOverflow = VerticalWrapMode.Overflow;
                        text.color = Color.white;
                        text.GetComponent<RectTransform>().localScale = Vector3.one * .02f;
                        debugText = text;
                    }
                }
            }
            catch (Exception e)
            {
                Logging.Exception(e);
            }
        }

        void OnEnable()
        {
            try
            {
                Logging.Debug("OnEnable");
                this.pluginEnabled = true;
                HarmonyPatches.ApplyHarmonyPatches();
                if (initialized)
                    Setup();
            }
            catch (Exception e)
            {
                Logging.Exception(e);
            }
        }

        void OnDisable()
        {
            try
            {
                Logging.Debug("OnDisable");
                this.pluginEnabled = false;
                HarmonyPatches.RemoveHarmonyPatches();
                Cleanup();
            }
            catch (Exception e)
            {
                Logging.Exception(e);
            }
        }

        void OnGameInitialized()
        {
            try
            {
                Logging.Debug("OnGameInitialized");
                initialized = true;
                PlatformTagJoin platform = (PlatformTagJoin)Traverse.Create(GorillaNetworking.PlayFabAuthenticator.instance).Field("platform").GetValue();
                Logging.Info("Platform: ", platform);
                IsSteam = platform.PlatformTag == "Steam";

                NetworkSystem.Instance.OnJoinedRoomEvent += asdadasewwfdsfsf;
                NetworkSystem.Instance.OnReturnedToSinglePlayer += roomJoined;

                if (DebugMode)
                    CreateDebugGUI();
            }
            catch (Exception ex)
            {
                Logging.Exception(ex);
            }
        }

        private void roomJoined()
        {
            if (WaWa_graze_dot_cc)
            {
                ModdedJoin();
            }
        }

        private void asdadasewwfdsfsf()
        {
            qwertyujikl();

            // if (NetworkSystem.Instance.GameModeString.Contains("MODDED_"))
            // {
            //     qwertyujikl();
            // }
        }

        void qwertyujikl()
        {
            Logging.Debug("RoomJoined");
            WaWa_graze_dot_cc = true;
            Setup();
        }

        //this isnt rly a join, it's to break "cracks"
        //I will keep renaming whatever i have to :3
        void ModdedJoin()
        {
            Logging.Debug("RoomLeft");
            WaWa_graze_dot_cc = false;
            Cleanup();
        }

        public void JoinLobby(string name, string gamemode)
        {
            StartCoroutine(JoinLobbyInternal(name, gamemode));
        }

        IEnumerator JoinLobbyInternal(string name, string gamemode)
        {
            NetworkSystem.Instance.ReturnToSinglePlayer();
            do
            {
                yield return new WaitForSeconds(1f);
                Logging.Debug("Waiting to disconnect");
            }
            while (PhotonNetwork.InRoom);

            string gamemodeCache = GorillaComputer.instance.currentGameMode.Value;
            Logging.Debug("Changing gamemode from", gamemodeCache, "to", gamemode);
            GorillaComputer.instance.currentGameMode.Value = gamemode;
            PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(name, JoinType.Solo);

            while (!PhotonNetwork.InRoom)
            {
                yield return new WaitForSeconds(1f);
                Logging.Debug("Waiting to connect");
            }
            GorillaComputer.instance.currentGameMode.Value = gamemodeCache;
        }
    }
}
