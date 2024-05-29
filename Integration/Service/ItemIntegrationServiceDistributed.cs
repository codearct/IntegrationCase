using Integration.Backend;
using Integration.Common;

namespace Integration.Service;
public class ItemIntegrationServiceDistributed
{
    private ItemOperationBackend ItemIntegrationBackend { get; set; } = new();


    //need for read and write-lock mechanism
    //firstly need for read-lock.if suceess,we need for write-lock
    //write-lock must be within read-lock.
    //if they seperated,when one thread is in write-lock after read-lock,
    //other thread with same content would capture read-lock
    //it will cause unnecessary database request
    //and second thread pass write-block even if it cannot write
    //lockKey must be unique.
    //because of item content uniqueness,item content can be lockKey
    public async Task<Result> SaveItem(string itemContent)
    {
        var expireTime = TimeSpan.FromMilliseconds(2_000);//Would be 40 seconds
        string lockKeyForRead = $"Read_{itemContent}";

        List<Item> items;
        Item item = null;

        try
        {
            //Assume that Local machine has Redis installed.
            //StackExchange library used for redis implementations.
            bool isLockedForRead = RedisHelper.RedisLock(lockKeyForRead, itemContent, expireTime);

            if (isLockedForRead)
            {
                items = await ItemIntegrationBackend.FindItemsWithContent(itemContent);
                if (items.Count != 0)
                {                   
                    return new Result(false, $"Duplicate item received with content {itemContent}.");
                }

                string lockKeyForWrite = $"Write_{itemContent}";
                try
                {
                    bool isLockedForWrite = RedisHelper.RedisLock(lockKeyForWrite, itemContent, expireTime);
                    if(isLockedForWrite)
                    {
                        item = await ItemIntegrationBackend.SaveItem(itemContent);
                    }
                    else
                    {
                        return new Result(false, $"Retry for item received with content {itemContent}.");
                    }
                }
                finally
                {
                    RedisHelper.RedisLockFree(lockKeyForWrite);
                }
                return new Result(true, $"Item with content {itemContent} saved with id {item.Id}.");
            }
            else
            {
                return new Result(false, $"Retry for item received with content {itemContent}.");
            }          
        }
        finally
        {
            RedisHelper.RedisLockFree(lockKeyForRead);
        };
    }

    public List<Item> GetAllItems()
    {
        return ItemIntegrationBackend.GetAllItems();
    }
}
