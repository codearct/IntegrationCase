using Integration.Common;
using Integration.Backend;
using System.Diagnostics;

namespace Integration.Service;

//Focused on just thread-safe locking
public sealed class ItemIntegrationService
{
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();
    private ReaderWriterLockSlim readerWriterLock = new();
    public async Task<Result> SaveItem(string itemContent)
    {
        List<Item> items;
        Item item = null;

        try
        {
            //need for read and write-lock mechanism
            //firstly need for read-lock.if suceess,we need for write-lock
            //write-lock must be within read-lock.
            //if they seperated,when one thread is in write-lock after read-lock,
            //other thread with same content would capture read-lock
            //it will cause unnecessary database request
            //and second thread pass write-block even if it cannot write
            readerWriterLock.EnterUpgradeableReadLock();
            items = await ItemIntegrationBackend.FindItemsWithContent(itemContent);
            if (items.Count != 0)
            {
                return new Result(false, $"Duplicate item received with content {itemContent}.");
            }

            try
            {
                readerWriterLock.EnterWriteLock();
                item = await ItemIntegrationBackend.SaveItem(itemContent);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }

            return new Result(true, $"Item with content {itemContent} saved with id {item.Id}.");
        }
        finally
        {
            readerWriterLock.ExitUpgradeableReadLock();           
        };
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}