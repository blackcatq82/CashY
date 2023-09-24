namespace CashY.Services
{
    public interface IIPermissionServices
    {
        Task<bool> GetRequestPermissionStorageRead();
        Task<bool> GetRequestPermissionStorageWrite();
        Task<bool> GetRequestPermissionPhotos();
        Task<bool> GetRequestPermissionNetworking();
    }


    public class IPermissionsServices : IIPermissionServices
    {
        public async Task<bool> GetRequestPermissionStorageRead()
        {
            try
            {
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status == PermissionStatus.Granted)
                    return await Task.FromResult(true);

                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // Prompt the user to turn on in settings
                    // On iOS once a permission has been denied it may not be requested again from the application
                    return await Task.FromResult(false);
                }

                if (Permissions.ShouldShowRationale<Permissions.StorageRead>())
                {
                    // Prompt the user with additional information as to why the permission is needed
                }

                status = await Permissions.RequestAsync<Permissions.StorageRead>();
                return await Task.FromResult(status == PermissionStatus.Granted);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Permission Service Error:" + ex.Message);
            }

            return await Task.FromResult(false);
        }
        public async Task<bool> GetRequestPermissionStorageWrite()
        {
            try
            {
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                if (status == PermissionStatus.Granted)
                    return await Task.FromResult(true);

                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // Prompt the user to turn on in settings
                    // On iOS once a permission has been denied it may not be requested again from the application
                    return await Task.FromResult(false);
                }

                if (Permissions.ShouldShowRationale<Permissions.StorageWrite>())
                {
                    // Prompt the user with additional information as to why the permission is needed
                }

                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                return await Task.FromResult(status == PermissionStatus.Granted);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Permission Service Error:" + ex.Message);
            }

            return await Task.FromResult(false);
        }
        public async Task<bool> GetRequestPermissionPhotos()
        {
            try
            {
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Photos>();
                if (status == PermissionStatus.Granted)
                    return await Task.FromResult(true);

                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // Prompt the user to turn on in settings
                    // On iOS once a permission has been denied it may not be requested again from the application
                    return await Task.FromResult(false);
                }

                if (Permissions.ShouldShowRationale<Permissions.Photos>())
                {
                    // Prompt the user with additional information as to why the permission is needed
                }

                status = await Permissions.RequestAsync<Permissions.Photos>();
                return await Task.FromResult(status == PermissionStatus.Granted);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Permission Service Error:" + ex.Message);
            }

            return await Task.FromResult(false);
        }
        public async Task<bool> GetRequestPermissionNetworking()
        {
            try
            {
                PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.NetworkState>();
                if (status == PermissionStatus.Granted)
                    return await Task.FromResult(true);

                if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
                {
                    // Prompt the user to turn on in settings
                    // On iOS once a permission has been denied it may not be requested again from the application
                    return await Task.FromResult(false);
                }

                if (Permissions.ShouldShowRationale<Permissions.NetworkState>())
                {
                    // Prompt the user with additional information as to why the permission is needed
                }

                status = await Permissions.RequestAsync<Permissions.NetworkState>();
                return await Task.FromResult(status == PermissionStatus.Granted);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Permission Service Error:" + ex.Message);
            }

            return await Task.FromResult(false);
        }
    }
}
