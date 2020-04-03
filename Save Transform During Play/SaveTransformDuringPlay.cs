#define DEBUG_LV1_ // Debug All
#define DEBUG_LV2_ // Debug Parent -> Child Order
#define DEBUG_LV3_ // Debug Global Scale

using UnityEngine;

namespace Rito.Conveniences
{
    /// <summary> 
    /// <para/> 제작일 : 2020. 04. 01. 
    /// <para/> 제작자 : Rito
    /// <para/> 기능   : 플레이 모드에서 Transform을 변경한 경우, 플레이 모드가 종료되어도 변경사항을 적용할 수 있게 합니다.
    /// <para/> 
    /// <para/> * 사용법
    /// <para/>  - 원하는 게임오브젝트에 SaveTransformDuringPlay 컴포넌트를 추가합니다.
    /// <para/>  - 인스펙터에서 On 변수를 체크/해제함으로써 언제든 기능을 켜고 끌 수 있습니다.
    /// <para/>  - 인스펙터의 Position, Rotation, Scale Space 옵션을 설정할 수 있습니다.
    /// <para/>
    /// <para/>    1) Local Space인 경우, 플레이 모드 종료 직전의 인스펙터 값을 그대로 보존합니다.
    /// <para/>       (transform.localPosition, transform.localRotation, transform.localScale)
    /// <para/>
    /// <para/>    2) World Space인 경우, 플레이 모드 종료 직전의 실제 위치, 회전, 크기 값을 그대로 보존합니다.
    /// <para/>       (transform.position, transform.rotation, transform.lossyScale)
    /// <para/> 
    /// </summary>
    [ExecuteInEditMode]
    public class SaveTransformDuringPlay : MonoBehaviour
    {
        #region Fields

        /// <summary> 기능 On/Off </summary>
        public bool _on = true;

        [Header("Options")]
        public PositionSpace _positionSpace = default;
        public RotationSpace _rotationSpace = default;
        public ScaleSpace    _scaleSpace    = default;

        private const int True  = 1;
        private const int False = 0;
        private const int World = 1;
        private const int Local = 0;

        #endregion

        #region Unity Callbacks

        private void OnGUI()
        {
            OnGUI_EditorOnly();
        }

        private void OnApplicationQuit()
        {
            OnApplicationQuit_EditorOnly();
        }

        #endregion

        #region EditorOnly Unity Callbacks

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnGUI_EditorOnly()
        {
            if (!Application.isPlaying)
            {
#if DEBUG_LV1
            Debug.Log($"{name} : OnGUI(), Editor Mode");
#endif
                // 부모 -> 자식 순서대로 차근차근 적용(World Space 역순 적용 문제 해결)
                if (transform.parent != null)
                {
                    var parentTr = transform.parent;
                    var parentSTDP = parentTr.GetComponent<SaveTransformDuringPlay>();
                    if(parentSTDP != null && parentSTDP._on)
                    {
                        if (CheckPrefs(nameof(Prefs._OnEditor), False, parentTr))
                        {
#if DEBUG_LV2
                            Debug.Log($"{name} : 대기 - 부모 번호 : {parentTr.transform.GetInstanceID()}");
#endif
                            return;
                        }
                    }
                }

                // OnEditorMode() 1회만 실행
                if (CheckPrefs(nameof(Prefs._OnEditor), False))
                {
                    OnEditorMode();
                    SavePrefs(nameof(Prefs._OnEditor), True);
                }
#if DEBUG_LV2
                Debug.Log($"{name} : _OnEditor : {CheckPrefs(nameof(Prefs._OnEditor), True)} - 번호 : {transform.GetInstanceID()}");
                if(transform.parent != null)
                    Debug.Log($"{name} : 내 번호 : {transform.GetInstanceID()}, 부모 번호 : {transform.parent.transform.GetInstanceID()}");
#endif
            }
        }

        // 플레이모드가 종료되는 순간
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnApplicationQuit_EditorOnly()
        {
#if DEBUG_LV1
        Debug.Log($"{name} : OnApplicationQuit()");
#endif
            // 기능이 동작 중이라면 JSON으로 저장
            if (_on)
            {
                JsonManager.SaveDataToJSON(new TransformData(transform), transform.GetInstanceID());
            }

            // 종료 순간의 인스펙터 값들 저장
            SaveInspectorOptionsToJSON();

            // OnEditor() 1회 호출용
            SavePrefs(nameof(Prefs._OnEditor), False);
        }

        #endregion

        #region Methods

        // 에디터모드가 실행되는 순간 1회 호출
        private void OnEditorMode()
        {
#if DEBUG_LV2
            Debug.Log($"{name} : OnEditorMode()");
#endif
#if DEBUG_LV1
        Debug.Log($"{name} : OnEditorMode()");
#endif
            // 저장했던 인스펙터 옵션들 다시 로드
            LoadInspectorOptionsFromJSON();

            // 기능 활성화 상태라면 트랜스폼 변경사항 적용
            if (_on)
                ApplyModifications();
        }

        private void ApplyModifications()
        {
#if DEBUG_LV1
        Debug.Log($"{name} : ApplyModifications()");
#endif
            var savedData = (JsonManager.LoadDataFromJSON<TransformData>(transform.GetInstanceID())) as TransformData;

            if (savedData == null || savedData.Equals(CommonData.Null) || !enabled || !_on)
                return;

            savedData.ApplyToTransform(transform, _positionSpace, _rotationSpace, _scaleSpace);

            // Scale - Global인 경우 적용
            if(_scaleSpace.Equals(ScaleSpace.World))
            {
                Vector3 savedLossy = savedData.globalScale;
                Vector3 nowLossy = transform.lossyScale;

                (float x, float y, float z) factor
                    = (savedLossy.x / nowLossy.x, savedLossy.y / nowLossy.y, savedLossy.z / nowLossy.z);

#if DEBUG_LV3
                Debug.Log($"Saved : {savedLossy}");
                Debug.Log($"Now   : {nowLossy}");
                Debug.Log($"Factor : {factor}");
#endif

                transform.localScale = new Vector3(
                        transform.localScale.x * factor.x,
                        transform.localScale.y * factor.y,
                        transform.localScale.z * factor.z
                    );
            }
        }

        #endregion

        #region Tiny Methods

        private void SaveInspectorOptionsToJSON()
        {
            SpaceData sd = new SpaceData(_on, _positionSpace, _rotationSpace, _scaleSpace);
            JsonManager.SaveDataToJSON(sd, transform.GetInstanceID());
        }

        private void LoadInspectorOptionsFromJSON()
        {
            var savedOptions = JsonManager.LoadDataFromJSON<SpaceData>(transform.GetInstanceID()) as SpaceData;

            if (savedOptions == null || savedOptions.Equals(CommonData.Null))
                return;

            _on = savedOptions.isOn;
            _positionSpace = savedOptions.positionSpace;
            _rotationSpace = savedOptions.rotationSpace;
            _scaleSpace = savedOptions.scaleSpace;
        }

        private void SavePrefs(in string prefName, in int answer)
        {
#if DEBUG_LV1
        Debug.Log($$"{name} : SavePrefs({prefName})");
#endif
            PlayerPrefs.SetInt(transform.GetInstanceID() + prefName, answer);
        }

        private bool CheckPrefs(in string prefName, in int answer)
        {
#if DEBUG_LV1
        Debug.Log($$"{name} : CheckPrefs({prefName})");
#endif
            return PlayerPrefs.GetInt(transform.GetInstanceID() + prefName).Equals(answer);
        }

        private bool CheckPrefs(in string prefName, in int answer, in Transform targetTr)
        {
            return PlayerPrefs.GetInt(targetTr.GetInstanceID() + prefName).Equals(answer);
        }

        private enum Prefs
        {
            /// <summary> 플레이 모드 종료 후, 에디터 모드 실행됨 </summary>
            _OnEditor
        }

        #endregion
    }
}