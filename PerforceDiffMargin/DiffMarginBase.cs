using System;
using System.Windows;
using System.Windows.Controls;
using PerforceDiffMargin.ViewModel;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;

namespace PerforceDiffMargin
{
    internal abstract class DiffMarginBase : Canvas, IWpfTextViewMargin
    {
        protected readonly ITextView TextView;
        private bool _isDisposed;
        protected DiffMarginViewModelBase ViewModel;
        protected UserControl UserControl;

        protected abstract string MarginName { get; }

        protected DiffMarginBase(ITextView textView)
        {
            TextView = textView;

            TextView.Options.OptionChanged += HandleOptionChanged;
            TextView.LayoutChanged += OnLayoutChanged;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _isDisposed = true;
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return UserControl.ActualWidth;
            }
        }

        public bool Enabled
        {
            get
            {
                return TextView.Options.IsSelectionMarginEnabled();
            }
        }

        public FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return UserControl;
            }
        }

        private void HandleOptionChanged(object sender, EditorOptionChangedEventArgs e)
        {
            if (!_isDisposed && e.OptionId == PerforceDiffMarginTextViewOptions.DiffMarginName)
            {
                UpdateVisibility();
            }
        }

        private void UpdateVisibility()
        {
            ThrowIfDisposed();
            UserControl.Visibility = Enabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            ViewModel.RefreshDiffViewModelPositions();
        }
    }
}