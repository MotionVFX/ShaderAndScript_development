using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class FX_HotkeySwitcher : MonoBehaviour
{
    public List<StuffObject> StuffObjects = new List<StuffObject>();
    private int i;
    private int j;


    void DisableObjects()
    {
        for (i = 0; i < (StuffObjects.Count); i++)
            for (j = 0; j < (StuffObjects[i].objectsArray.Count); j++)
                StuffObjects[i].objectsArray[j].obj.SetActive(false);
    }

    void EnableObjectsByNumber(int Number)
    {
        for (j = 0; j < (StuffObjects[Number].objectsArray.Count); j++)
            StuffObjects[i].objectsArray[j].obj.SetActive(true);
    }


    public void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            DisableObjects();
            EnableObjectsByNumber(0);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            DisableObjects();
            EnableObjectsByNumber(1);
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            DisableObjects();
            EnableObjectsByNumber(2);
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            DisableObjects();
            EnableObjectsByNumber(3);
        }
    }

    [System.Serializable]
    public class StuffObject
    {
        public List<objectArray> objectsArray = new List<objectArray>();
    }

    [System.Serializable]
    public class objectArray
    {
        public GameObject obj;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(FX_HotkeySwitcher))]
    public class HotkeySwitcherEditor : Editor
    {
        private FX_HotkeySwitcher ps;
        private int counter = 1;
        private int i;

        public void OnEnable()
        {
            ps = (FX_HotkeySwitcher)target;
        }

        public override void OnInspectorGUI()
        {
            // Чтоб рисовали актуальную версию объекта
            serializedObject.Update();
            if (ps.StuffObjects.Count > 0)
            {
                foreach (StuffObject obj in ps.StuffObjects)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        ps.StuffObjects.Remove(obj);
                        break;
                    }
                    if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(20))) obj.objectsArray.Add(new objectArray());

                    EditorGUILayout.EndHorizontal();

                    if (obj.objectsArray.Count > 0)
                    {
                        foreach (objectArray objArray in obj.objectsArray)
                        {
                            EditorGUILayout.BeginHorizontal("box");
                            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)))
                            {
                                obj.objectsArray.Remove(objArray);
                                break;
                            }
                            objArray.obj = (GameObject)EditorGUILayout.ObjectField("Объект", objArray.obj, typeof(GameObject), true);
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    else EditorGUILayout.LabelField("Нет элементов в списке");
                    EditorGUILayout.EndVertical();
                }
            }

            else EditorGUILayout.LabelField("Нет элементов в списке");
            if (GUILayout.Button("Добавить элемент", GUILayout.Height(25))) ps.StuffObjects.Add(new StuffObject());

            //проверка на плей мод, что бы не сохранять изменения в плей моде
            if (!Application.isPlaying)
                if (GUI.changed) Save(ps.gameObject);

            serializedObject.ApplyModifiedProperties();
        }

        public static void Save(GameObject obj)
        {
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(obj.scene);
        }
    }

#endif

}