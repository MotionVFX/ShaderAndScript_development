using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class FX_PrefabSwitcher : MonoBehaviour
{
    public List<StuffObject> StuffObjects;

    private int switchObj = 0;
    private float curveValue = 0;
    private int i;
    private float defaultValue;


    //анимация по кривой
    void ChangeObjectParametrs()
    {
        //если текстовая переменная не указана, то нет смысла считать кривую
        if (StuffObjects[switchObj].shaderVarName != null & StuffObjects[switchObj].obj != null)
        {
            if (StuffObjects[switchObj].counter <= StuffObjects[switchObj].lifeTime)
            {
                var timeProjection = StuffObjects[switchObj].counter / StuffObjects[switchObj].lifeTime;
                curveValue = StuffObjects[switchObj].shaderVarValue.Evaluate(timeProjection);

                for (i = 0; i < (StuffObjects[switchObj].rends.Length); i++)
                    StuffObjects[switchObj].rends[i].material.SetFloat(StuffObjects[switchObj].shaderVarName, curveValue);
            }
        }
    }

    //вернуть дефолтное значение
    void ReturnDefaultParametrs()
    {
        //не нужно возвращать дефолтное значение если переменная не менялась
        if (StuffObjects[switchObj].shaderVarName != null & StuffObjects[switchObj].obj != null)
        {
            //дефолтное значение берем из кривой (value из time = 0)
            defaultValue = StuffObjects[switchObj].shaderVarValue.Evaluate(0.0f);

            for (i = 0; i < (StuffObjects[switchObj].rends.Length); i++)
                StuffObjects[switchObj].rends[i].material.SetFloat(StuffObjects[switchObj].shaderVarName, defaultValue);
        }
    }


    void Update()
    {
        StuffObjects[switchObj].counter += Time.deltaTime;
        //вызываем функцию с анимацией по кривой
        ChangeObjectParametrs();

        if (StuffObjects[switchObj].counter >= StuffObjects[switchObj].lifeTime)
        {
            //выключаем объект, так как его выремя проигрывания закончилось
            if (StuffObjects[switchObj].obj != null)
                StuffObjects[switchObj].obj.SetActive(false);

            //после того как выключили объект, возвращаем дефолтные значения
            //переменной, которая анимировалась кривой
            ReturnDefaultParametrs();

            if (switchObj < StuffObjects.Count - 1)
            {
                switchObj++;

                if (StuffObjects[switchObj].obj != null)
                {
                    //включаем следующий объект
                    StuffObjects[switchObj].obj.SetActive(true);
                    //находим рендеры всех детей нашего объекта, что бы потом, в кривой менять переменную в шейдере
                    StuffObjects[switchObj].rends = StuffObjects[switchObj].obj.GetComponentsInChildren<Renderer>();
                }
            }

            else
            {
                for (int i = 0; i < StuffObjects.Count; i++)
                {
                    StuffObjects[i].counter = 0;
                }

                curveValue = 0;
                switchObj = 0;
                StuffObjects[0].obj.SetActive(true);
                StuffObjects[switchObj].rends = StuffObjects[switchObj].obj.GetComponentsInChildren<Renderer>();
            }
        }
    }

    [System.Serializable]
    public class StuffObject
    {
        public GameObject obj;
        public float lifeTime;
        public string shaderVarName;
        public AnimationCurve shaderVarValue;
        public float counter = 0;
        public Renderer[] rends;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(FX_PrefabSwitcher))]
    public class PrefabSwitcherEditor : Editor
    {
        private FX_PrefabSwitcher ps;

        public void OnEnable()
        {
            ps = (FX_PrefabSwitcher)target;
        }

        public override void OnInspectorGUI()
        {
            if (ps.StuffObjects.Count > 0)
            {
                foreach (StuffObject obj in ps.StuffObjects)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.Space();
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        ps.StuffObjects.Remove(obj);
                        break;
                    }
                    obj.obj = (GameObject)EditorGUILayout.ObjectField("Отслеживаемый объект", obj.obj, typeof(GameObject), true);
                    obj.lifeTime = EditorGUILayout.FloatField("Время жизни", obj.lifeTime);
                    obj.shaderVarName = EditorGUILayout.TextField("Имя переменной шейдера", obj.shaderVarName);
                    obj.shaderVarValue = EditorGUILayout.CurveField("Кривая", obj.shaderVarValue);
                    EditorGUILayout.EndVertical();
                }
            }

            else EditorGUILayout.LabelField("Нет элементов в списке");
            if (GUILayout.Button("Добавить элемент", GUILayout.Height(25))) ps.StuffObjects.Add(new StuffObject());
            if (GUI.changed) Save(ps.gameObject);
        }

        public static void Save(GameObject obj)
        {
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(obj.scene);
        }
    }

#endif
}
