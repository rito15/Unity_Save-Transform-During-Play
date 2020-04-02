#define USE_DEBUG_

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

        private bool _onEditorMode = false;

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
#if USE_DEBUG
            Debug.Log("Method Call : OnGUI(), Editor Mode");
#endif
                if (_onEditorMode == false)
                {
                    OnEditorMode();
                    _onEditorMode = true;
                }

                // 플레이모드가 종료되고, 에디터모드가 시작된 순간에 트랜스폼 변경사항 적용
                if (_on && CheckPrefs(nameof(Prefs._stApplied), False))
                    ApplyModifications();
            }
        }

        // 플레이모드가 종료되는 순간 JSON으로 저장
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnApplicationQuit_EditorOnly()
        {
#if USE_DEBUG
        Debug.Log("Method Call : OnApplicationQuit()");
#endif
            if (_on)
            {
                JsonTransformDataManager.SaveTransformDataToJSON(new TransformData(transform), GetInstanceID());
                SavePrefs(nameof(Prefs._stApplied), False);
            }

            SavePrefs(nameof(Prefs._stOn), _on ? True : False);
            SavePrefs(nameof(Prefs._stPosSpace), _positionSpace.Equals(PositionSpace.World) ? World : Local);
            SavePrefs(nameof(Prefs._stRotSpace), _rotationSpace.Equals(RotationSpace.World) ? World : Local);
        }

        #endregion

        #region Methods

        // 에디터모드가 실행되는 순간 호출
        private void OnEditorMode()
        {
#if USE_DEBUG
        Debug.Log("Method Call : OnEditorMode()");
#endif
            // 플레이 도중 _on 변수를 바꾼 경우
            if (_on.XOR(CheckPrefs(nameof(Prefs._stOn), True)))
            {
                _on = !_on;
            }

            if(_positionSpace.Equals(PositionSpace.Local).XOR(CheckPrefs(nameof(Prefs._stPosSpace), Local)))
            {
#if USE_DEBUG
                Debug.Log("Pos : " + _positionSpace + " | " + (CheckPrefs(nameof(Prefs._stPosSpace), Local) ? "Local" : "Global"));
#endif
                _positionSpace.Reverse();
            }

            if(_rotationSpace.Equals(RotationSpace.Local).XOR(CheckPrefs(nameof(Prefs._stRotSpace), Local)))
            {
                _rotationSpace.Reverse();
            }
        }

        private void ApplyModifications()
        {
#if USE_DEBUG
        Debug.Log("Method Call : ApplyModifications()");
#endif
            TransformData savedData = JsonTransformDataManager.LoadTransformDataFromJSON(GetInstanceID());

            if (savedData.Equals(TransformData.Null) || savedData.isApplied || !enabled || !_on)
                return;

            savedData.Load(transform, _positionSpace, _rotationSpace, _scaleSpace);
            savedData.isApplied = true;
            JsonTransformDataManager.SaveTransformDataToJSON(savedData, GetInstanceID());

            SavePrefs(nameof(Prefs._stApplied), _on ? True : False);
        }

#endregion

        #region Tiny Methods

        private void SavePrefs(in string prefName, in int answer)
        {
#if USE_DEBUG
        Debug.Log($"Method Call : SavePrefs({prefName})");
#endif
            PlayerPrefs.SetInt(GetInstanceID() + prefName, answer);
        }

        private bool CheckPrefs(in string prefName, int answer)
        {
#if USE_DEBUG
        Debug.Log($"Method Call : CheckPrefs({prefName})");
#endif
            return PlayerPrefs.GetInt(GetInstanceID() + prefName).Equals(answer);
        }

        private enum Prefs
        {
            /// <summary> Transform 변경사항이 성공적으로 적용됨 </summary>
            _stApplied,
            /// <summary> 기능 사용 여부 </summary>
            _stOn,

            _stPosSpace,
            _stRotSpace,
        }

        #endregion
    }
}