using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Blend.Optimizely
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class ContentEventHandler : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var eventRegistry = ServiceLocator.Current.GetInstance<IContentEvents>();

            eventRegistry.CreatingContent += OnCreatingContent;
            eventRegistry.CreatedContent += OnCreatedContent;
            eventRegistry.DeletingContent += OnDeletingContent;
            eventRegistry.DeletedContent += OnDeletedContent;
            eventRegistry.SavingContent += OnSavingContent;
            eventRegistry.SavedContent += OnSavedContent;
            eventRegistry.PublishingContent += OnPublishingContent;
            eventRegistry.PublishedContent += OnPublishedContent;
            eventRegistry.MovingContent += OnMovingContent;
            eventRegistry.MovedContent += OnMovedContent;
        }

        private void OnCreatingContent(object? sender, ContentEventArgs e)
        {
            if (e.Content is ICreatingContentHandler creatingContent)
            {
                creatingContent.CreatingContent(sender, e);
            }
        }

        private void OnCreatedContent(object? sender, ContentEventArgs e)
        {
            if (e.Content is ICreatedContentHandler createdContent)
            {
                createdContent.CreatedContent(sender, e);
            }
        }

        private void OnDeletingContent(object? sender, DeleteContentEventArgs e)
        {
            if (e.Content is IDeletingContentHandler deletingContent)
            {
                deletingContent.DeletingContent(sender, e);
            }
        }

        private void OnDeletedContent(object? sender, DeleteContentEventArgs e)
        {
            if (e.Content is IDeletedContentHandler deletedContent)
            {
                deletedContent.DeletedContent(sender, e);
            }
        }

        private void OnSavingContent(object? sender, ContentEventArgs e)
        {
            if (e.Content is ISavingContentHandler savingContent)
            {
                savingContent.SavingContent(sender, e);
            }
        }

        private void OnSavedContent(object? sender, ContentEventArgs e)
        {
            if (e.Content is ISavedContentHandler savedContent)
            {
                savedContent.SavedContent(sender, e);
            }
        }

        private void OnPublishingContent(object? sender, ContentEventArgs e)
        {
            if (e.Content is IPublishingContentHandler publishingContent)
            {
                publishingContent.PublishingContent(sender, e);
            }
        }

        private void OnPublishedContent(object? sender, ContentEventArgs e)
        {
            if (e.Content is IPublishedContentHandler publishedContent)
            {
                publishedContent.PublishedContent(sender, e);
            }
        }

        private void OnMovingContent(object? sender, ContentEventArgs e)
        {
            if (e.Content is IMovingContentHandler movingContent)
            {
                movingContent.MovingContent(sender, e);
            }
        }

        private void OnMovedContent(object? sender, ContentEventArgs e)
        {
            if (e.Content is IMovedContentHandler movedContent)
            {
                movedContent.MovedContent(sender, e);
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            var eventRegistry = ServiceLocator.Current.GetInstance<IContentEvents>();

            eventRegistry.CreatingContent -= OnCreatingContent;
            eventRegistry.CreatedContent -= OnCreatedContent;
            eventRegistry.DeletingContent -= OnDeletingContent;
            eventRegistry.DeletedContent -= OnDeletedContent;
            eventRegistry.SavingContent -= OnSavingContent;
            eventRegistry.SavedContent -= OnSavedContent;
            eventRegistry.PublishingContent -= OnPublishingContent;
            eventRegistry.PublishedContent -= OnPublishedContent;
            eventRegistry.MovingContent -= OnMovingContent;
            eventRegistry.MovedContent -= OnMovedContent;
        }
    }

    public interface ICreatedContentHandler
    {
        void CreatedContent(object? sender, ContentEventArgs e);
    }

    public interface ICreatingContentHandler
    {
        void CreatingContent(object? sender, ContentEventArgs e);
    }

    public interface IDeletedContentHandler
    {
        void DeletedContent(object? sender, DeleteContentEventArgs e);
    }

    public interface IDeletingContentHandler
    {
        void DeletingContent(object? sender, DeleteContentEventArgs e);
    }

    public interface IPublishedContentHandler
    {
        void PublishedContent(object? sender, ContentEventArgs e);
    }

    public interface IPublishingContentHandler
    {
        void PublishingContent(object? sender, ContentEventArgs e);
    }

    public interface ISavedContentHandler
    {
        void SavedContent(object? sender, ContentEventArgs e);
    }

    public interface ISavingContentHandler
    {
        void SavingContent(object? sender, ContentEventArgs e);
    }

    public interface IMovingContentHandler
    {
        void MovingContent(object? sender, ContentEventArgs e);
    }

    public interface IMovedContentHandler
    {
        void MovedContent(object? sender, ContentEventArgs e);
    }
}