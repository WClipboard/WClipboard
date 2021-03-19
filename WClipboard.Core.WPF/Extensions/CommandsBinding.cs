using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WClipboard.Core.Utilities;
using WClipboard.Core.WPF.Models;
using WClipboard.Core.WPF.ViewModels;

namespace WClipboard.Core.WPF.Extensions
{
    public static class CommandsBinding
    {
        public static readonly DependencyProperty InputBindingsProperty = DependencyProperty.RegisterAttached("InputBindings", typeof(IList), typeof(CommandsBinding), new PropertyMetadata(OnInputBindingsChanged), IsValidInputBindings);
        public static readonly DependencyProperty InteractableInputBindingsProperty = DependencyProperty.RegisterAttached("InteractableInputBindings", typeof(Interactable), typeof(CommandsBinding), new PropertyMetadata(OnInteractableInputBindingsPropertyChanged));
        private static readonly DependencyProperty InputBindingsHelperProperty = DependencyProperty.RegisterAttached("InputBindingsHelper", typeof(NotifyCollectionChangedEventHandler), typeof(CommandsBinding));

        private static void SetInputBindingsHelper(UIElement target, NotifyCollectionChangedEventHandler? value)
        {
            target.SetValue(InputBindingsHelperProperty, value);
        }

        private static NotifyCollectionChangedEventHandler? GetInputBindingsHelper(UIElement target)
        {
            return (NotifyCollectionChangedEventHandler?)target.GetValue(InputBindingsHelperProperty);
        }

        public static void SetInputBindings(UIElement target, IList value)
        {
            target.SetValue(InputBindingsProperty, value);
        }

        public static IList GetInputBindings(UIElement target)
        {
            return (IList)target.GetValue(InputBindingsProperty);
        }

        public static void SetInteractableInputBindings(UIElement target, Interactable value)
        {
            target.SetValue(InteractableInputBindingsProperty, value);
        }

        public static Interactable GetInteractableInputBindings(UIElement target)
        {
            return (Interactable)target.GetValue(InteractableInputBindingsProperty);
        }

        public static void OnInputBindingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if(!(sender is UIElement uiElement))
            {
                return;
            }


            if (e.NewValue != e.OldValue)
            {
                if (e.OldValue is INotifyCollectionChanged oV)
                {
                    oV.CollectionChanged -= GetInputBindingsHelper(uiElement);
                    SetInputBindingsHelper(uiElement, null);
                }

                if (e.NewValue is INotifyCollectionChanged nV)
                {
                    var weakUiElementRef = new WeakReference<UIElement>(uiElement);

                    var handler = new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs e) =>
                    {
                        if (weakUiElementRef.TryGetTarget(out var uiElement))
                        {
                            UpdateInputBindings(uiElement);
                        }
                    });

                    SetInputBindingsHelper(uiElement, handler);
                    nV.CollectionChanged += handler;
                }

                UpdateInputBindings(uiElement);
            }
        }

        public static void OnInteractableInputBindingsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UIElement target))
            {
                return;
            }

            if (e.NewValue != e.OldValue)
            {
                target.InputBindings.Clear();
                var interactable = GetInteractableInputBindings(target);
                if(interactable != null)
                {
                    var dataContext = TryFindIHasInteractablesInTree(target);
                    foreach (var interactableAction in interactable.Actions.Where(ia => ia.MouseGesture != null))
                    {
                        target.InputBindings.Add(new InputBinding(interactableAction, interactableAction.MouseGesture) { CommandParameter = dataContext });
                    }
                }
            }
        }

        private static IHasInteractables? TryFindIHasInteractablesInTree(UIElement element)
        {
            return (
                RecursiveEnumerable.Get((DependencyObject?)element, e => VisualTreeHelper.GetParent(e), null)
                    .FirstOrDefault(e => e is FrameworkElement fe && fe.DataContext is IHasInteractables) 
                as FrameworkElement)?.DataContext as IHasInteractables;
        }

        public static bool IsValidInputBindings(object inputBindings)
        {
            if(inputBindings == null)
            {
                return true;
            }
            return inputBindings.GetType()
                .GetInterfaces()
                .Any(t => t.IsGenericType
                    && t.GetGenericTypeDefinition() == typeof(IList<>)
                    && typeof(InteractableState).IsAssignableFrom(t.GetGenericArguments()[0]));
        }

        private static void UpdateInputBindings(UIElement target)
        {
            var keyInteractableActions = GetInputBindings(target);

            target.InputBindings.Clear();
            if (keyInteractableActions != null)
            {
                var dataContext = (target as FrameworkElement)?.DataContext;
                foreach (var interactableAction in keyInteractableActions.Cast<InteractableState>().SelectMany(i => i.Interactable.Actions).Where(ia => ia.KeyGesture != null))
                {
                    target.InputBindings.Add(new InputBinding(interactableAction, interactableAction.KeyGesture) { CommandParameter = dataContext });
                }
            }
        }

        public static readonly DependencyProperty CommandBindingsProperty = DependencyProperty.RegisterAttached("CommandBindings", typeof(IList), typeof(CommandsBinding), new PropertyMetadata(OnCommandBindingsChanged), IsValidCommandBindings);
        private static readonly DependencyProperty CommandBindingsHelperProperty = DependencyProperty.RegisterAttached("CommandBindingsHelper", typeof(NotifyCollectionChangedEventHandler), typeof(CommandsBinding));

        private static void SetCommandBindingsHelper(UIElement target, NotifyCollectionChangedEventHandler? value)
        {
            target.SetValue(CommandBindingsHelperProperty, value);
        }

        private static NotifyCollectionChangedEventHandler? GetCommandBindingsHelper(UIElement target)
        {
            return (NotifyCollectionChangedEventHandler?)target.GetValue(CommandBindingsHelperProperty);
        }

        public static void SetCommandBindings(UIElement target, IList value)
        {
            target.SetValue(CommandBindingsProperty, value);
        }

        public static IList GetCommandBindings(UIElement target)
        {
            return (IList)target.GetValue(CommandBindingsProperty);
        }

        public static void OnCommandBindingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is UIElement uiElement))
            {
                return;
            }

            if (e.NewValue != e.OldValue)
            {
                if (e.OldValue is INotifyCollectionChanged oV)
                {
                    oV.CollectionChanged -= GetCommandBindingsHelper(uiElement);
                    SetCommandBindingsHelper(uiElement, null);
                }

                if (e.NewValue is INotifyCollectionChanged nV)
                {
                    var weakUiElementRef = new WeakReference<UIElement>(uiElement);

                    var handler = new NotifyCollectionChangedEventHandler((object sender, NotifyCollectionChangedEventArgs e) =>
                    {
                        if(weakUiElementRef.TryGetTarget(out var uiElement))
                        {
                            UpdateCommandBindings(uiElement);
                        }
                    });

                    SetCommandBindingsHelper(uiElement, handler);
                    nV.CollectionChanged += handler;
                }

                UpdateCommandBindings(uiElement);
            }
        }

        public static bool IsValidCommandBindings(object CommandBindings)
        {
            if (CommandBindings == null)
            {
                return true;
            }
            return CommandBindings.GetType()
                .GetInterfaces()
                .Any(t => t.IsGenericType
                    && t.GetGenericTypeDefinition() == typeof(IList<>)
                    && typeof(CommandBinding).IsAssignableFrom(t.GetGenericArguments()[0]));
        }

        private static void UpdateCommandBindings(UIElement target)
        {
            var commandBindings = GetCommandBindings(target);

            target.CommandBindings.Clear();
            if (commandBindings != null)
            {
                foreach (var commandBinding in commandBindings.Cast<CommandBinding>())
                {
                    target.CommandBindings.Add(commandBinding);
                }
            }
        }
    }
}
