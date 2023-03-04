# Localized WinForms

## Overview

### Introduction

To my big surprise, I found it difficult to get a decent explanation and code on how to localize (make multilingual) a Windows Forms application.

After hours of research and tests, I found a way and share it here.

### Features

This is a dumb empty WinForms with these characteristics:

- Built from the Windows Forms App template,
- .NET Framework 4.7.2,
- C# language,
- Simple Form with:
  - a Main form (Form1),
  - a ComboBox to switch language (comboBox1),
  - a Quit button that will adapt to the language (button1),

- When changing the language in the comboBox, the UI language changes:

  - English UI (Default):

  - Switch Language with ComboBox:

  - French UI:

This is the ideal starter for anything else and more complex.

### Disclaimer

This code is delivered "as-is". It lacks many production features and recommended patterns, like Unit Testing, Dependency Injection, Factory, Logging, etc.

It just focuses on getting the Language UI to change.

And, of course, can be greatly improved and complexified.

## Steps-by-steps

### 1. Create the project

  !["Template selection"](/media/10_CreateProjectSolution.png)

### 2. Adapt the Form UI

- Make the Form smaller

- Add a Button (from Toolbox, drag-n-drop):

    !["Add button"](/media/30_AddButton.png)

- Add a ComboBox (from Toolbox):

    !["Add ComboBox"](/media/40_AddComboBox.png)

Result so far:

  !["Form1"](/media/50_Form1.png)

### 3. Setup the ComboBox

The ComboBox allows to change the language of the Form.

The ComboBox content can be managed in 2 ways:

- Option 1: With an Items `Collection` directly in the ComboBox object,

- Option 2: Through a DataBinding to an external source.

The `Option 1 - Items collection` is easy. When you understand how ComboBox operates, you can easily manipulate the [ComboBox.Items](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.items?view=windowsdesktop-7.0) and [ComboBox.SelectedIndex](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox.selectedindex?view=windowsdesktop-7.0) properties with code.

For more flexibility, I decided to use `Option 2 - DataBinding` for these  reasons:

- The DataSource binding can be changed easily manipulated through code,

- The source can be of any type and even dynamic.  

#### ComboBox Items

- Create a folder `Classes` and a class `Lang` (so we don't conflict with the Built-in Class):

  ```C#
  namespace WindowsFormsApp1.Classes
  {
      internal class Lang
      {
          public int Index { get; }
          public string Name { get; }
          public string Code { get; }

          public Lang(int index, string name, string code)
          {
              Index = index;
              Name = name;
              Code = code;
          }
      }
  }
  ```

  Remarks:
  - It appears an `integer` is required for the ComboBox to function. It is used as the `Index` by the ComboBox,
  - `Name` property will be set to be what is displayed in the Form,
  - `Code` will allow us to switch the Thread Culture in a secure manner.

- Open `Form1.cs`. It should contain only this:

    ```C#
    namespace WindowsFormsApp1
    {
        public partial class Form1 : Form
        {
            public Form1()
            {
                InitializeComponent();
            }
        }
    }
    ```

- Add code to it to gather the Languages values:

  - 1 private member of type `IDictionary<string, Lang[]>` that will store the `LanguagesCollections` used by the ComboBox,

  - 1 private method that will populate the `LanguagesCollections`. Here we will hard-code, but it can come from any source,

  - Add the method the method to be executed in `Form1()` constructor. It binds the member to the Form1 lifecycle.

  It gives:

  ```C#
  public Form1()
  {
      InitializeComponent();
      GetLanguagesCollections();
  }

  private IDictionary<string, Lang[]> _languages;

  private void GetLanguagesCollections()
  {
      //These are hard-coded here, but could be pulled from any settings source, Dependency Injected, and set as global constants for the entire App, if more than 1 Win Form is used.
      _languages = new Dictionary<string, Lang[]>
      {
          {
              "en-US",
              new[]
      {
          new Lang(0, "English", "en-US"),
          new Lang(1, "French", "fr-FR"),
      }
          },
          {
              "fr-FR",
              new[]
      {
          new Lang(0, "Anglais", "en-US"),
          new Lang(1, "Français", "fr-FR"),
      }
          }
      };
  }
  ```

  Remarks:
  - I used an `Array` instead of a `List<Language>` because it is pretty static in nature.
  - The use of a `IDictionary<string, Language[]>` will allow to the code generic.

- Add code to `Form1.cs` to bind the Languages values to the ComboBox:

  ```C#
  private void SetComboBoxItemsLanguage(string lang)
  {
      comboBox1.DataSource = _languages[lang];
      comboBox1.DisplayMember = "Name";
      comboBox1.ValueMember = "Code";
      var index = Array.Find(_languages[lang], element => element.Code == lang).Index;
      comboBox1.SelectedIndex = index;
  }
  ```

- Generate code for the `Form1_Load` event:

  - Go to the tab `Form1.cs [Design]`

  - Check the entire `Form1` is selected

  - Switch from `Properties` to `Events`:
  
    !["Switch to Events"](/media//60_SwitchToEvents.png)

  - Double-Click in the `Load` event

    !["DoubleClick Load"](/media/70_FormLoad.png)

  - Check you are in the `Form1_Load` event code:

    !["Form1_Load event code"](/media/80_FormLoad_method.png)

  - Add this command to the event:

    ```C#
    private void Form1_Load(object sender, EventArgs e)
    {
      SetComboBoxItemsLanguage(_languages.First().Key);
    }
    ```
  Now, the ComboBox can load the languages items and change them depending of the requested language.

#### ComboBox Items change to the selected Language

  - Go to the tab `Form1.cs [Design]`

  - Click on the `comboBox1` to select it

  - Switch from `Properties` to `Events`:

  - Double click in `SelectionChangeCommitted` event to create the event's code:

    !["SelectionChangeCommitted"](/media/90_comboBox1_ChangeCommitted.png)

    !["event code"](/media/100_changeCommitted_event_code.png)

  - Add this code to adapt the comboBox Items:

  ```C#
  private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
  {
      if (!string.IsNullOrEmpty(comboBox1.SelectedValue.ToString()))
      {
          SetComboBoxItemsLanguage(comboBox1.SelectedValue.ToString());
      }
      else
      {
          SetComboBoxItemsLanguage("en-US");
      }
  }
  ```

  Now, the Form runs, and when the Language is changed, the language list in the ComboBox changes also:

  !["combo EN"](/media/110_comboBox_EN.png) !["Combo FR"](/media/120_comboBox_FR.png)

### 4. Make the Form UI multilingual




## References

Here are few articles that helped me through the process:

[ComboBox Control Overview (Windows Forms)](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/controls/combobox-control-overview-windows-forms?view=netframeworkdesktop-4.8)

[ComboBox Class](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.combobox?view=windowsdesktop-7.0)

[Programmatically Binding DataSource To ComboBox In Multiple Ways](https://www.c-sharpcorner.com/UploadFile/0f68f2/programmatically-binding-datasource-to-combobox-in-multiple/)