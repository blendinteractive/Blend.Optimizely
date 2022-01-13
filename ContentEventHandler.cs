using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Blend.Episerver
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

        private void OnCreatingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is ICreatingContentHandler)
            {
                ((ICreatingContentHandler)e.Content).CreatingContent(sender, e);
            }
        }

        private void OnCreatedContent(object sender, ContentEventArgs e)
        {
            if (e.Content is ICreatedContentHandler)
            {
                ((ICreatedContentHandler)e.Content).CreatedContent(sender, e);
            }
        }

        private void OnDeletingContent(object sender, DeleteContentEventArgs e)
        {
            if (e.Content is IDeletingContentHandler)
            {
                ((IDeletingContentHandler)e.Content).DeletingContent(sender, e);
            }
        }

        private void OnDeletedContent(object sender, DeleteContentEventArgs e)
        {
            if (e.Content is IDeletedContentHandler)
            {
                ((IDeletedContentHandler)e.Content).DeletedContent(sender, e);
            }
        }

        private void OnSavingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is ISavingContentHandler)
            {
                ((ISavingContentHandler)e.Content).SavingContent(sender, e);
            }
        }

        private void OnSavedContent(object sender, ContentEventArgs e)
        {
            if (e.Content is ISavedContentHandler)
            {
                ((ISavedContentHandler)e.Content).SavedContent(sender, e);
            }
        }

        private void OnPublishingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is IPublishingContentHandler)
            {
                ((IPublishingContentHandler)e.Content).PublishingContent(sender, e);
            }
        }

        private void OnPublishedContent(object sender, ContentEventArgs e)
        {
            if (e.Content is IPublishedContentHandler)
            {
                ((IPublishedContentHandler)e.Content).PublishedContent(sender, e);
            }
        }

        private void OnMovingContent(object sender, ContentEventArgs e)
        {
            if (e.Content is IMovingContentHandler)
            {
                ((IMovingContentHandler)e.Content).MovingContent(sender, e);
            }
        }

        private void OnMovedContent(object sender, ContentEventArgs e)
        {
            if (e.Content is IMovedContentHandler)
            {
                ((IMovedContentHandler)e.Content).MovedContent(sender, e);
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

        public void Preload(string[] parameters)
        {
        }
    }

    public interface ICreatedContentHandler
    {
        void CreatedContent(object sender, ContentEventArgs e);
    }

    public interface ICreatingContentHandler
    {
        void CreatingContent(object sender, ContentEventArgs e);
    }

    public interface IDeletedContentHandler
    {
        void DeletedContent(object sender, DeleteContentEventArgs e);
    }

    public interface IDeletingContentHandler
    {
        void DeletingContent(object sender, DeleteContentEventArgs e);
    }

    public interface IPublishedContentHandler
    {
        void PublishedContent(object sender, ContentEventArgs e);
    }

    public interface IPublishingContentHandler
    {
        void PublishingContent(object sender, ContentEventArgs e);
    }

    public interface ISavedContentHandler
    {
        void SavedContent(object sender, ContentEventArgs e);
    }

    public interface ISavingContentHandler
    {
        void SavingContent(object sender, ContentEventArgs e);
    }

    public interface IMovingContentHandler
    {
        void MovingContent(object sender, ContentEventArgs e);
    }

    public interface IMovedContentHandler
    {
        void MovedContent(object sender, ContentEventArgs e);
    }
}