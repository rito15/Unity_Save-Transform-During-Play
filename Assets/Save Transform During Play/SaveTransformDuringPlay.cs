#define USE_DEBUG_

using UnityEngine;

// 2020. 04. 01. 작성
// 제작자 : Rito
// 기능   : 플레이 모드에서 Transform을 변경한 경우, 플레이 모드가 종료되어도 변경사항을 적용할 수 있게 합니다.
// 사용법 : 원하는 게임오브젝트에 SaveTransformDuringPlay 컴포넌트를 추가합니다.
//          인스펙터에서 On 변수를 체크/해제함으로써 언제든 기능을 켜고 끌 수 있습니다.

namespace Rito.Conveniences
{
    [ExecuteInEditMode]
    public class SaveTransformDuringPlay : MonoBehaviour
    {
        /// <summary> 기능 On/Off </summary>
        public bool _on = true;

        private const int Yes = 1;
        private const int No = 0;

        private bool _onEditorMode = false;

        private void OnGUI()
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
                if (_on && CheckPrefs(nameof(Prefs._tmApplied), No))
                    ApplyModifications();
            }
        }

        // 플레이모드가 종료되는 순간 JSON으로 저장
        private void OnApplicationQuit()
        {
#if USE_DEBUG
        Debug.Log("Method Call : OnApplicationQuit()");
#endif
            if (_on)
            {
                JsonTransformDataManager.SaveTransformDataToJSON(new TransformData(transform), GetInstanceID());
                SavePrefs(nameof(Prefs._tmApplied), No);
            }

            SavePrefs(nameof(Prefs._tmaOn), _on ? Yes : No);
        }

        // 에디터모드가 실행되는 순간 호출
        private void OnEditorMode()
        {
#if USE_DEBUG
        Debug.Log("Method Call : OnEditorMode()");
#endif
            // 플레이 도중 _on : false -> true로 바꾼 경우
            if (!_on && CheckPrefs(nameof(Prefs._tmaOn), Yes))
            {
                _on = true;

#if USE_DEBUG
            Debug.Log("플레이 도중 기능 ON");
#endif
            }

            // 플레이 도중 _on : true -> false 바꾼 경우
            else if (_on && CheckPrefs(nameof(Prefs._tmaOn), No))
            {
                _on = false;

#if USE_DEBUG
            Debug.Log("플레이 도중 기능 OFF");
#endif
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

            savedData.Load(transform);
            SavePrefs(nameof(Prefs._tmApplied), Yes);

            savedData.isApplied = true;
            JsonTransformDataManager.SaveTransformDataToJSON(savedData, GetInstanceID());
        }


        private void SavePrefs(in string prefName, in int yesOrNo)
        {
#if USE_DEBUG
        Debug.Log($"Method Call : SavePrefs({prefName})");
#endif
            PlayerPrefs.SetInt(GetInstanceID() + prefName, yesOrNo);
        }

        private bool CheckPrefs(in string prefName, int yesOrNo)
        {
#if USE_DEBUG
        Debug.Log($"Method Call : CheckPrefs({prefName})");
#endif
            return PlayerPrefs.GetInt(GetInstanceID() + prefName).Equals(yesOrNo);
        }

        private enum Prefs
        {
            /// <summary> Transform 변경사항이 성공적으로 적용됨 </summary>
            _tmApplied,
            /// <summary> 기능 사용 여부 </summary>
            _tmaOn
        }
    }
}