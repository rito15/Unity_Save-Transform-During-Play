#define DEBUG_LV1_ // Debug All
#define DEBUG_LV2_

using UnityEngine;

/*

 작성일 : 2020. 04. 01. 
 제작자 : Rito
 기능   : 플레이 모드에서 Transform을 변경한 경우, 플레이 모드가 종료되어도 변경사항을 적용할 수 있게 합니다.

 * 사용법
  - 원하는 게임오브젝트에 SaveTransformDuringPlay 컴포넌트를 추가합니다.
  - 인스펙터에서 On 변수를 체크/해제함으로써 언제든 기능을 켜고 끌 수 있습니다.
  - 인스펙터의 positionSpace를 설정하여 플레이모드 해제 시 localPosition 또는 globalPosition 중 하나를 선택하여 적용할 수 있습니다.
  - 인스펙터의 rotationSpace를 설정하여 플레이모드 해제 시 localRotation 또는 globalRotation 중 하나를 선택하여 적용할 수 있습니다.

*/
namespace Rito.Conveniences
{
    [ExecuteInEditMode]
    public class SaveTransformDuringPlay : MonoBehaviour
    {
        #region Fields

        /// <summary> 기능 On/Off </summary>
        public bool _on = true;

        [Header("Options")]
        public PositionSpace _positionSpace = default;
        public RotationSpace _rotationSpace = default;

        private ScaleSpace   _scaleSpace    = default; // TODO : lossy

        private const int True  = 1;
        private const int False = 0;
        private const int World = 1;
        private const int Local = 0;

        #endregion

        #region Unity Callbacks

        private void Reset()
        {
            _on = true;
        }

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
                if (transform.parent != null && transform.parent.gameObject != null)
                {
                    var parentTr = transform.parent;
                    var parentSTDP = parentTr.GetComponent<SaveTransformDuringPlay>();
                    if(parentSTDP != null && parentSTDP._on)
                    {
                        if (CheckPrefs(nameof(Prefs._OnEditor), False, parentTr.gameObject))
                        {
#if DEBUG_LV2
                            Debug.Log($"{name} : 대기 - 부모 번호 : {parentTr.gameObject.GetInstanceID()}");
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
                Debug.Log($"{name} : _OnEditor : {CheckPrefs(nameof(Prefs._OnEditor), True)} - 번호 : {gameObject.GetInstanceID()}");
                if(transform.parent != null)
                    Debug.Log($"{name} : 내 번호 : {gameObject.GetInstanceID()}, 부모 번호 : {transform.parent.gameObject.GetInstanceID()}");
#endif
            }
        }

        // 플레이모드가 종료되는 순간 JSON으로 저장
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnApplicationQuit_EditorOnly()
        {
#if DEBUG_LV1
        Debug.Log($"{name} : OnApplicationQuit()");
#endif
            if (_on)
            {
                JsonManager.SaveTransformDataToJSON(new TransformData(transform), GetInstanceID());
            }

            // 종료 순간의 인스펙터 값들 저장
            SavePrefs(nameof(Prefs._stOn), _on ? True : False);
            SavePrefs(nameof(Prefs._stPosSpace), _positionSpace.Equals(PositionSpace.World) ? World : Local);
            SavePrefs(nameof(Prefs._stRotSpace), _rotationSpace.Equals(RotationSpace.World) ? World : Local);

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

            // 플레이 도중 _on 변수를 바꾼 경우
            if (_on.XOR(CheckPrefs(nameof(Prefs._stOn), True)))
            {
                _on = !_on;
            }

            if(_positionSpace.Equals(PositionSpace.Local).XOR(CheckPrefs(nameof(Prefs._stPosSpace), Local)))
            {
#if DEBUG_LV1
                Debug.Log("Pos : " + _positionSpace + " | " + (CheckPrefs(nameof(Prefs._stPosSpace), Local) ? "Local" : "Global"));
#endif
                _positionSpace.Reverse();
            }

            if(_rotationSpace.Equals(RotationSpace.Local).XOR(CheckPrefs(nameof(Prefs._stRotSpace), Local)))
            {
                _rotationSpace.Reverse();
            }


            // 기능 활성화 상태라면 트랜스폼 변경사항 적용
            if (_on)
                ApplyModifications();
        }

        private void ApplyModifications()
        {
#if DEBUG_LV1
        Debug.Log($"{name} : ApplyModifications()");
#endif
            TransformData savedData = JsonManager.LoadTransformDataFromJSON(GetInstanceID());

            if (savedData.Equals(TransformData.Null) || !enabled || !_on)
                return;

            savedData.Load(transform, _positionSpace, _rotationSpace, _scaleSpace);
            JsonManager.SaveTransformDataToJSON(savedData, GetInstanceID());
        }

#endregion

        #region Tiny Methods

        private void SavePrefs(in string prefName, in int answer)
        {
#if DEBUG_LV1
        Debug.Log($$"{name} : SavePrefs({prefName})");
#endif
            PlayerPrefs.SetInt(gameObject.GetInstanceID() + prefName, answer);
        }

        private bool CheckPrefs(in string prefName, in int answer)
        {
#if DEBUG_LV1
        Debug.Log($$"{name} : CheckPrefs({prefName})");
#endif
            return PlayerPrefs.GetInt(gameObject.GetInstanceID() + prefName).Equals(answer);
        }

        private bool CheckPrefs(in string prefName, in int answer, in GameObject targetGO)
        {
            return PlayerPrefs.GetInt(targetGO.GetInstanceID() + prefName).Equals(answer);
        }

        private enum Prefs
        {
            /// <summary> 플레이 모드 종료 후, 에디터 모드 실행됨 </summary>
            _OnEditor,

            /// <summary> 기능 사용 여부 </summary>
            _stOn,

            _stPosSpace,
            _stRotSpace,
        }

        #endregion
    }
}