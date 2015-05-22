#### One Minute GUI

[One Minute GUI](https://www.assetstore.unity3d.com/en/#!/content/32346) is a really easy to use and well designed presentation layer. I personally use it in my projects for casual games (Like [GhostHunter VR](http://ghosthuntervr.com)). Here are some steps to integrate it with Unity3d Databinding.

- I used a single ViewModel for the entire project.
- IsVisible and transition logic was delegated to OMG.
- If a part of the code was complex, I partended it into a sub model using ObservableObject.
- I used a [Custom MenuManager](https://github.com/NVentimiglia/Unity3d-Databinding-Mvvm-Mvc/blob/master/OneMinuteGUI/MenuManager.cs) which provided better programmic access.
- [Example MainViewModel for One Minute GUI](https://github.com/NVentimiglia/Unity3d-Databinding-Mvvm-Mvc/blob/master/OneMinuteGUI/GameViewModel.cs)
