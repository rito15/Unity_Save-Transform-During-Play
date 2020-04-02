# Save Transform During Play
 ### [1] 설명
  - 기존에는 플레이 모드에서 트랜스폼의 위치, 회전, 크기를 변경하여도
    <br>플레이 모드가 종료되면 변경사항이 저장되지 않습니다.
    
  - 따라서 컴포넌트를 간단히 추가하기만 하면 플레이 모드를 종료해도
    <br>트랜스폼의 변경사항이 저장되도록 하는 컴포넌트를 제작했습니다.
  
  <br>
  
 ### [2] 사용법
  - 원하는 게임오브젝트에 ```SaveTransformDuringPlay``` 컴포넌트를 추가하고, ```On```에 체크합니다.
  
  - 인스펙터에서 ```On``` 변수를 체크 또는 해제함으로써 언제든 기능을 켜고 끌 수 있습니다.
  
  - 인스펙터에서 ```Position Space```를 ```Local``` 또는 ```World```로 설정하여,
    <br> 플레이모드가 종료될 때 localPosition과 globalPosition 중 어떤 것을 유지할지 선택할 수 있습니다.
  
  - 인스펙터에서 ```Rotation Space```를 ```Local``` 또는 ```World```로 설정하여,
    <br> 플레이모드가 종료될 때 localRotation과 globalRotation 중 어떤 것을 유지할지 선택할 수 있습니다.
  
  <br>
  
 ### [3] 사용 예시
  - .
  
  <br>
