using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Text.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace App34
{
    public sealed partial class MyEditBox : UserControl
    {

        CoreTextEditContext _editContext;

        CoreWindow _coreWindow;
        bool _internalFocus = false;
        InputPane _inputPane;

        int _currentLine = 0;

        List<TextItem> _content;

        public string GetTextFromContent()
        {
            string text = "";
            foreach (var line in _content)
            {
                if (line is TodoTextItem)
                {
                    text += "todo: " + line.Text + '\n';
                }
                else
                {
                    text += line.Text + '\n';
                }
            }

            return text;
        }

        // If the _selection starts and ends at the same point,
        // then it represents the location of the caret (insertion point).
        CoreTextRange _selection;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MyEditBox), new PropertyMetadata(null, TextChanged));

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MyEditBox editBox)
            {
                List<TextItem> items = new List<TextItem>();

                foreach (var line in editBox.Text.Split('\n'))
                {
                    if (line.Trim().ToLower().StartsWith("todo:"))
                    {
                        items.Add(new TodoTextItem()
                        {
                            Text = line.Trim().Substring(5).Trim()
                        });
                    }
                    else
                    {
                        items.Add(new TextItem()
                        {
                            Text = line
                        });
                    }
                }

                editBox._content = items;
                editBox._currentLine = 0;

                CoreTextRange range;
                range.StartCaretPosition = range.EndCaretPosition = 0;

                editBox.SetSelectionAndNotify(range);

                editBox.RenderContent();
            }

        }

        public MyEditBox()
        {
            this.InitializeComponent();

            _coreWindow = CoreWindow.GetForCurrentThread();
            _coreWindow.KeyDown += CoreWindow_KeyDown;
            _coreWindow.PointerPressed += CoreWindow_PointerPressed;

            CoreTextServicesManager manager = CoreTextServicesManager.GetForCurrentView();
            _editContext = manager.CreateEditContext();

            // Get the Input Pane so we can programmatically hide and show it.
            _inputPane = InputPane.GetForCurrentView();

            _editContext.InputScope = CoreTextInputScope.Text;
            _editContext.TextRequested += EditContext_TextRequested;

            // The system raises this event to request the current selection.
            _editContext.SelectionRequested += EditContext_SelectionRequested;

            // The system raises this event when it wants the edit control to remove focus.
            _editContext.FocusRemoved += EditContext_FocusRemoved;

            // The system raises this event to update text in the edit control.
            _editContext.TextUpdating += EditContext_TextUpdating;

            // The system raises this event to change the selection in the edit control.
            _editContext.SelectionUpdating += EditContext_SelectionUpdating;

            // The system raises this event to request layout information.
            // This is used to help choose a position for the IME candidate window.
            _editContext.LayoutRequested += EditContext_LayoutRequested;

            // The system raises this event to notify the edit control
            // that the string composition has started.
            _editContext.CompositionStarted += EditContext_CompositionStarted;

            // The system raises this event to notify the edit control
            // that the string composition is finished.
            _editContext.CompositionCompleted += EditContext_CompositionCompleted;
        }

        private void RenderContent()
        {
            CustomEditRoot.Children.Clear();

            var root = new StackPanel();

            if (_content != null)
            {
                for (int i = 0; i < _content.Count; i++)
                {
                    var line = _content[i];
                    FrameworkElement text;

                    if (_currentLine == i)
                    {
                        text = renderTextWithCaret(line.Text);
                    }
                    else
                    {
                        text = new TextBlock()
                        {
                            Text = line.Text,
                            TextWrapping = TextWrapping.WrapWholeWords,
                            FontSize = 14
                        };
                    }

                    if (line is TodoTextItem item)
                    {
                        root.Children.Add(renderTodoItem(text, item));
                    }
                    else
                    {
                        root.Children.Add(text);
                    }
                }
            }

            CustomEditRoot.Children.Add(root);
        }

        FrameworkElement renderTextWithCaret(string text)
        {
            var textBlock = new TextBlock()
            {
                TextWrapping = TextWrapping.WrapWholeWords
            };

            textBlock.Inlines.Add(new Run()
            {
                Text = PreserveTrailingSpaces(text.Substring(0, _selection.StartCaretPosition))
            });

            if (_internalFocus)
            {
                textBlock.Inlines.Add(new Run()
                {
                    FontSize = 22,
                    Text = "I",
                });
            }

            textBlock.Inlines.Add(new Run()
            {
                Text = PreserveTrailingSpaces(text.Substring(_selection.EndCaretPosition))
            });


            return textBlock;
        }

        FrameworkElement renderTodoItem(FrameworkElement content, TodoTextItem item)
        {
            var stack = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            stack.Children.Add(content);
            stack.Children.Add(new Image()
            {
                Source = new BitmapImage(new Uri("ms-appx:///Assets/todo.png")),
                Height = 10,
                Margin = new Thickness() { Left = 10}
            });

            var check = new CheckBox()
            {
                Content = stack
            };

            return check;
        }

        void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            // See whether the pointer is inside or outside the control.
            Rect contentRect = GetElementRect(CustomEditRoot);
            if (contentRect.Contains(args.CurrentPoint.Position))
            {
                // The user tapped inside the control. Give it focus.
                SetInternalFocus();

                // Tell XAML that this element has focus, so we don't have two
                // focus elements. That is the extent of our integration with XAML focus.
                Focus(FocusState.Programmatic);

                // A more complete custom control would move the caret to the
                // pointer position. It would also provide some way to select
                // text via touch. We do neither in this sample.

            }
            else
            {
                // The user tapped outside the control. Remove focus.
                RemoveInternalFocus();
            }
        }

        void SetInternalFocus()
        {
            if (!_internalFocus)
            {
                // Update internal notion of focus.
                _internalFocus = true;

                // Update the UI.
                UpdateTextUI();
                UpdateFocusUI();

                // Notify the CoreTextEditContext that the edit context has focus,
                // so it should start processing text input.
                _editContext.NotifyFocusEnter();
            }

            // Ask the software keyboard to show.  The system will ultimately decide if it will show.
            // For example, it will not show if there is a keyboard attached.
            _inputPane.TryShow();

        }

        void RemoveInternalFocus()
        {
            if (_internalFocus)
            {
                //Notify the system that this edit context is no longer in focus
                _editContext.NotifyFocusLeave();

                RemoveInternalFocusWorker();
            }
        }

        void RemoveInternalFocusWorker()
        {
            //Update the internal notion of focus
            _internalFocus = false;

            // Ask the software keyboard to dismiss.
            _inputPane.TryHide();

            // Update our UI.
            UpdateTextUI();
            UpdateFocusUI();
        }

        void EditContext_FocusRemoved(CoreTextEditContext sender, object args)
        {
            RemoveInternalFocusWorker();
        }

        void ReplaceText(CoreTextRange modifiedRange, string text)
        {
            // Modify the internal text store.

            var line = _content[_currentLine].Text;

            _content[_currentLine].Text = line.Substring(0, modifiedRange.StartCaretPosition) +
              text +
              line.Substring(modifiedRange.EndCaretPosition);

            // Move the caret to the end of the replacement text.
            _selection.StartCaretPosition = modifiedRange.StartCaretPosition + text.Length;
            _selection.EndCaretPosition = _selection.StartCaretPosition;

            // Update the selection of the edit context.  There is no need to notify the system
            // of the selection change because we are going to call NotifyTextChanged soon.
            SetSelection(_selection);

            // Let the CoreTextEditContext know what changed.
            _editContext.NotifyTextChanged(modifiedRange, text.Length, _selection);
        }

        void SetSelection(CoreTextRange selection)
        {
            // Modify the internal selection.
            _selection = selection;

            //Update the UI to show the new selection.
            UpdateTextUI();
        }

        bool HasSelection()
        {
            return _selection.StartCaretPosition != _selection.EndCaretPosition;
        }

        // Change the selection and notify CoreTextEditContext of the new selection.
        void SetSelectionAndNotify(CoreTextRange selection)
        {
            SetSelection(selection);
            _editContext.NotifySelectionChanged(_selection);
        }

        void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            // Do not process keyboard input if the custom edit control does not
            // have focus.
            if (!_internalFocus)
            {
                return;
            }

            // This holds the range we intend to operate on, or which we intend
            // to become the new selection. Start with the current selection.
            CoreTextRange range = _selection;

            // For the purpose of this sample, we will support only the left and right
            // arrow keys and the backspace key. A more complete text edit control
            // would also handle keys like Home, End, and Delete, as well as
            // hotkeys like Ctrl+V to paste.
            //
            // Note that this sample does not properly handle surrogate pairs
            // nor does it handle grapheme clusters.

            switch (args.VirtualKey)
            {
                // Backspace
                case VirtualKey.Back:
                    if (range.StartCaretPosition == 0 && _content[_currentLine] is TodoTextItem todoItem)
                    {
                        // convert todo item to a regular item
                        _content[_currentLine] = new TextItem()
                        {
                            Text = "todo" + todoItem.Text
                        };

                        range.StartCaretPosition = range.EndCaretPosition = 4;
                        SetSelectionAndNotify(range);
                    }
                    else if (range.StartCaretPosition == 0 && _currentLine > 0)
                    {
                        _currentLine--;
                        range.StartCaretPosition = _content[_currentLine].Text.Length;
                        range.EndCaretPosition = range.StartCaretPosition;

                        _content[_currentLine].Text += _content[_currentLine + 1].Text;

                        _content.RemoveAt(_currentLine + 1);

                        SetSelectionAndNotify(range);
                    }
                    else
                    {
                        range.StartCaretPosition = Math.Max(0, range.StartCaretPosition - 1);
                        ReplaceText(range, "");
                    }
                    break;

                // Left arrow
                case VirtualKey.Left:
                    // There was no selection. Move the caret left one code unit if possible.
                    if (range.StartCaretPosition == 0 && _currentLine > 0)
                    {
                        // move to previous line
                        _currentLine--;
                        range.StartCaretPosition = _content[_currentLine].Text.Length;
                        range.EndCaretPosition = range.StartCaretPosition;
                    }
                    else
                    {
                        range.StartCaretPosition = Math.Max(0, range.StartCaretPosition - 1);
                        range.EndCaretPosition = range.StartCaretPosition;
                    }
                    SetSelectionAndNotify(range);
                    break;

                // Right arrow
                case VirtualKey.Right:
                    // There was no selection. Move the caret right one code unit if possible.
                    if (range.StartCaretPosition == _content[_currentLine].Text.Length && _currentLine < _content.Count - 1)
                    {
                        // move to next line
                        _currentLine++;
                        range.StartCaretPosition = 0;
                        range.EndCaretPosition = range.StartCaretPosition;
                    }
                    else
                    {
                        range.StartCaretPosition = Math.Min(_content[_currentLine].Text.Length, range.StartCaretPosition + 1);
                        range.EndCaretPosition = range.StartCaretPosition;
                    }
                    SetSelectionAndNotify(range);
                    break;

                case VirtualKey.Down:

                    if (_currentLine < _content.Count - 1)
                    {
                        //if (_content[_currentLine] is TodoTextItem item)
                        //{
                        //    item.Update();
                        //}

                        _currentLine++;
                        range.StartCaretPosition = Math.Min(_content[_currentLine].Text.Length, range.StartCaretPosition);
                        range.EndCaretPosition = range.StartCaretPosition;
                        SetSelectionAndNotify(range);
                    }

                    break;

                case VirtualKey.Up:

                    if (_currentLine > 0)
                    {
                        //if (_content[_currentLine] is TodoTextItem item)
                        //{
                        //    item.Update();
                        //}

                        _currentLine--;
                        range.StartCaretPosition = Math.Min(_content[_currentLine].Text.Length, range.StartCaretPosition);
                        range.EndCaretPosition = range.StartCaretPosition;
                        SetSelectionAndNotify(range);
                    }

                    break;

                case VirtualKey.Enter:

                    var line = _content[_currentLine].Text;

                    var left = line.Substring(0, _selection.StartCaretPosition);
                    var right = line.Substring(_selection.EndCaretPosition);

                    _content[_currentLine].Text = left;

                    _content.Insert(_currentLine + 1, new TextItem()
                    {
                        Text = right
                    });

                    if (_content[_currentLine] is TodoTextItem item)
                    {
                        item.Update();
                    }

                    _currentLine++;

                    range.StartCaretPosition = 0;
                    range.EndCaretPosition = range.StartCaretPosition;
                    SetSelectionAndNotify(range);

                    break;
            }
        }

        void EditContext_TextRequested(CoreTextEditContext sender, CoreTextTextRequestedEventArgs args)
        {
            CoreTextTextRequest request = args.Request;
            var text = _content[_currentLine].Text;
            request.Text = text.Substring(
                request.Range.StartCaretPosition,
                Math.Min(request.Range.EndCaretPosition, text.Length) - request.Range.StartCaretPosition);
        }

        void EditContext_SelectionRequested(CoreTextEditContext sender, CoreTextSelectionRequestedEventArgs args)
        {
            CoreTextSelectionRequest request = args.Request;
            request.Selection = _selection;
        }

        async void EditContext_TextUpdating(CoreTextEditContext sender, CoreTextTextUpdatingEventArgs args)
        {
            CoreTextRange range = args.Range;
            string newText = args.Text;
            CoreTextRange newSelection = args.NewSelection;

            var line = _content[_currentLine].Text;

            //// Modify the internal text store.
            _content[_currentLine].Text = line.Substring(0, range.StartCaretPosition) +
                newText +
                line.Substring(Math.Min(line.Length, range.EndCaretPosition));

            if (_content[_currentLine].Text.Trim().ToLower().StartsWith("todo:") && _content[_currentLine] is not TodoTextItem)
            {
                var todoItem = new TodoTextItem()
                {
                    Text = _content[_currentLine].Text.Trim().Substring(5)
                };
                // convert item to todo
                _content.RemoveAt(_currentLine);
                _content.Insert(_currentLine, todoItem);
                newSelection.StartCaretPosition = newSelection.EndCaretPosition = 0;

                // had to add this to make sure it works
                await Task.Delay(2);
                SetSelectionAndNotify(newSelection);
            }
            else
            {
                //// Modify the current selection.
                newSelection.EndCaretPosition = newSelection.StartCaretPosition;

                //// Update the selection of the edit context. There is no need to notify the system
                //// because the system itself changed the selection.
                SetSelection(newSelection);
            }

            
        }

        void EditContext_SelectionUpdating(CoreTextEditContext sender, CoreTextSelectionUpdatingEventArgs args)
        {
            // Set the new selection to the value specified by the system.
            CoreTextRange range = args.Selection;

            // Update the selection of the edit context. There is no need to notify the system
            // because the system itself changed the selection.
            SetSelection(range);
        }

        static Rect ScaleRect(Rect rect, double scale)
        {
            rect.X *= scale;
            rect.Y *= scale;
            rect.Width *= scale;
            rect.Height *= scale;
            return rect;
        }

        void EditContext_LayoutRequested(CoreTextEditContext sender, CoreTextLayoutRequestedEventArgs args)
        {
            //CoreTextLayoutRequest request = args.Request;

            //// Get the screen coordinates of the entire control and the selected text.
            //// This information is used to position the IME candidate window.

            //// First, get the coordinates of the edit control and the selection
            //// relative to the Window.
            //Rect contentRect = GetElementRect(ContentPanel);
            //Rect selectionRect = GetElementRect(SelectionText);

            //// Next, convert to screen coordinates in view pixels.
            //Rect windowBounds = Window.Current.CoreWindow.Bounds;
            //contentRect.X += windowBounds.X;
            //contentRect.Y += windowBounds.Y;
            //selectionRect.X += windowBounds.X;
            //selectionRect.Y += windowBounds.Y;

            //// Finally, scale up to raw pixels.
            //double scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

            //contentRect = ScaleRect(contentRect, scaleFactor);
            //selectionRect = ScaleRect(selectionRect, scaleFactor);

            //// This is the bounds of the selection.
            //// Note: If you return bounds with 0 width and 0 height, candidates will not appear while typing.
            //request.LayoutBounds.TextBounds = selectionRect;

            ////This is the bounds of the whole control
            //request.LayoutBounds.ControlBounds = contentRect;
        }

        // This indicates that an IME has started composition.  If there is no handler for this event,
        // then composition will not start.
        void EditContext_CompositionStarted(CoreTextEditContext sender, CoreTextCompositionStartedEventArgs args)
        {
        }

        void EditContext_CompositionCompleted(CoreTextEditContext sender, CoreTextCompositionCompletedEventArgs args)
        {
        }

        void UpdateFocusUI()
        {
            CustomEditRoot.BorderBrush = _internalFocus ? new SolidColorBrush(Windows.UI.Colors.Green) : null;
        }

        void UpdateTextUI()
        {
            // The raw materials we have are a string (_text) and information about
            // where the caret/selection is (_selection). We can render the control
            // any way we like.

            RenderContent();
        }

        static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform transform = element.TransformToVisual(null);
            Point point = transform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

        // Helper function to put a zero-width non-breaking space at the end of a string.
        // This prevents TextBlock from trimming trailing spaces.
        static string PreserveTrailingSpaces(string s)
        {
            return s + "\ufeff";
        }
    }
}
