# Save Transform During Play
 ### [1] 설명
  - 기존에는 플레이 모드에서 트랜스폼의 위치, 회전, 크기를 변경하여도
    <br>플레이 모드가 종료되면 변경사항이 저장되지 않습니다.
    
  - 따라서 컴포넌트를 간단히 추가하기만 하면 플레이 모드를 종료해도
    <br>트랜스폼의 변경사항이 저장되도록 하는 컴포넌트를 제작했습니다.
  
  <br>
  
 ### [2] 사용법
  - 원하는 게임오브젝트에 ```SaveTransformDuringPlay``` 컴포넌트를 추가하고, ```On```에 체크합니다.
  
  - 플레이 모드에서 ```On```, 각각의 ```Space``` 옵션을 수정해도 수정사항이 적용됩니다.
  
  - 인스펙터에서 ```Position Space```를 ```Local``` 또는 ```World```로 설정하여,
    <br> 플레이모드가 종료될 때 localPosition과 globalPosition 중 어떤 값을 보존할지 선택할 수 있습니다.
  
  - 인스펙터에서 ```Rotation Space```를 ```Local``` 또는 ```World```로 설정하여,
    <br> 플레이모드가 종료될 때 localRotation과 globalRotation 중 어떤 값을 보존할지 선택할 수 있습니다.
  
  - 인스펙터에서 ```Scale Space```를 ```Local``` 또는 ```World```로 설정하여,
    <br> 플레이모드가 종료될 때 localScale과 lossyScale 중 어떤 값을 보존할지 선택할 수 있습니다.
  
  <br>
  
 ### [3] 사용 예시
  - Local Space 옵션을 적용하는 경우
   : 종료하기 직전 인스펙터의 transform 요소 값들을 그대로 보존합니다.
   
  ![STDP_Local](https://user-images.githubusercontent.com/42164422/78341218-489c3480-75d2-11ea-8db4-0166786ce24b.gif)
  
  <br>

  - World Space 옵션을 적용하는 경우
   : 종료하기 직전 오브젝트의 실제 위치, 회전, 크기를 그대로 보존합니다.
   
   ![STDP_World](https://user-images.githubusercontent.com/42164422/78341235-4fc34280-75d2-11ea-9b6c-9571782bfcb7.gif)
  
  <br>

  - 플레이 모드에서 ```On``` 변수를 체크 해제하는 경우
   : 컴포넌트를 추가하지 않은 상태와 마찬가지로, 트랜스폼 보존 기능이 작동하지 않습니다.
   
   ![STDP_Reset](https://user-images.githubusercontent.com/42164422/78341253-55b92380-75d2-11ea-9916-a43a3afbbed4.gif)

