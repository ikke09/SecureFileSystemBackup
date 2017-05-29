using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileSystemBackup.Core.Guardians
{
    public class FileGuardian
    {
        private readonly ILog _logger;
        private const int LOCK_TIMEOUT = 5000;

        private FileGuardianConfiguration _configuration = FileGuardianConfiguration.Default;
        private readonly ReaderWriterLockSlim _configurationLocker = new ReaderWriterLockSlim();

        private readonly Dictionary<DirectoryInfo, HashSet<DirectoryInfo>> _dictSourceToTargets = new Dictionary<DirectoryInfo, HashSet<DirectoryInfo>>();
        private readonly Dictionary<DirectoryInfo, FileSystemWatcher> _dictSourceToWatcher = new Dictionary<DirectoryInfo, FileSystemWatcher>();
        private readonly ReaderWriterLockSlim _dictLocker = new ReaderWriterLockSlim();


        public FileGuardian(ILog Logger)
        {
            if (Logger == null)
                throw new ArgumentNullException(nameof(Logger));

            this._logger = Logger;
        }

        #region TryLoadConfiguration
        /// <summary>
        /// A thread-safe attempt to load a new configuration for the file guardian.
        /// </summary>
        /// <param name="NewConfiguration">The new configuration instance.</param>
        /// <returns><c>true</c> if the configuration could be successfully loaded, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">The new configuration is null.</exception>
        /// <exception cref="ArgumentException">The new configuration is not valid.</exception>
        public bool TryLoadConfiguration(FileGuardianConfiguration NewConfiguration)
        {
            if (NewConfiguration == null)
                throw new ArgumentNullException(nameof(NewConfiguration));
            if (!NewConfiguration.IsValid)
                throw new ArgumentException($"{nameof(NewConfiguration)} is not valid!");


            bool blnSuccess = false;

            try
            {
                _configurationLocker.EnterWriteLock();
                _configuration = NewConfiguration;
                _configurationLocker.ExitWriteLock();
                blnSuccess = true;
            }
            catch (LockRecursionException ex)
            {
                this._logger.ErrorFormat("An error occurred while entering lock recursion! Message: {0}", ex);
            }
            catch (ObjectDisposedException)
            {
                this._logger.Error("The configuration locker has already been disposed.");
            }
            catch (SynchronizationLockException)
            {
                this._logger.Fatal("The current thread has not entered the lock in write mode!");
            }
            catch (Exception ex)
            {
                this._logger.FatalFormat("An unexpected error occurred while trying to load the new configuration! Message: {0}", ex);
            }

            return blnSuccess;
        }
        #endregion

        private void RefreshHandles()
        {

        }

        private bool AddNewWatcher(DirectoryInfo SourceDirectory, DirectoryInfo TargetDirectory)
        {
            if (SourceDirectory == null)
                throw new ArgumentNullException(nameof(SourceDirectory));
            if (!SourceDirectory.Exists)
                throw new ArgumentException("Source directory does not exist!");
            if (TargetDirectory == null)
                throw new ArgumentNullException(nameof(TargetDirectory));

            bool blnSuccess = false;
            FileSystemWatcher watcher = new FileSystemWatcher();

            try
            {
                if (_dictLocker.TryEnterWriteLock(LOCK_TIMEOUT))
                {
                    HashSet<DirectoryInfo> lstTargets;

                    if (!_dictSourceToTargets.TryGetValue(SourceDirectory, out lstTargets))
                    {
                        lstTargets = new HashSet<DirectoryInfo>();
                        _dictSourceToTargets[SourceDirectory] = lstTargets;
                    }

                    lstTargets.Add(TargetDirectory);
                    _dictSourceToWatcher[TargetDirectory] = watcher;
                    _dictLocker.ExitWriteLock();

                    if (!TargetDirectory.Exists)
                        TargetDirectory.Create();

                    watcher.Path = SourceDirectory.FullName;
                    watcher.NotifyFilter = (NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName);

                    watcher.Changed += Watcher_Changed;
                    watcher.Created += Watcher_Created;
                    watcher.Deleted += Watcher_Deleted;
                    watcher.Renamed += Watcher_Renamed;

                    watcher.EnableRaisingEvents = true;

                    blnSuccess = true;
                }
                else
                {
                    this._logger.ErrorFormat("Watcher could not be added due to a lock timeout after {0}ms. ('{1}' -> '{2}')", LOCK_TIMEOUT, SourceDirectory, TargetDirectory);
                }
            }
            catch (LockRecursionException ex)
            {
                this._logger.ErrorFormat("An error occurred while entering lock recursion! Message: {0}", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                this._logger.ErrorFormat("Watcher could not be added due to an invalid parameter configuration! Message: {0}", ex);
            }
            catch (ArgumentNullException ex)
            {
                this._logger.ErrorFormat("Watcher could not be added due to an invalid parameter configuration! Message: {0}", ex);
            }
            catch (ObjectDisposedException ex)
            {
                this._logger.ErrorFormat("Watcher could not be added due to an disposed object! Message: {0}", ex);
            }
            catch (NullReferenceException ex)
            {
                this._logger.ErrorFormat("Watcher could not be added due to a missing lock instance! Thread synchronization is not ensured! Message: {0}", ex);
            }
            catch (SynchronizationLockException ex)
            {
                this._logger.FatalFormat("Watcher could not be added due the current thread not entering the lock in write mode! Message: {0}", ex);
            }
            catch (IOException ex)
            {
                this._logger.ErrorFormat("Watcher could not be added due to an error while creating the target directory! Message: {0}", ex);
            }
            catch (InvalidEnumArgumentException ex)
            {
                this._logger.ErrorFormat("Watcher could not be added due to an invalid filter type set for the watcher! Message: {0}", ex);
            }
            catch (PlatformNotSupportedException ex)
            {
                this._logger.FatalFormat("Watcher could not be added due to a not supported operating system! Windows NT or later is required! Message: {0}", ex);
            }
            catch (Exception ex)
            {
                this._logger.FatalFormat("Watcher could not be added due to an unexpected error! Message: {0}", ex);
            }

            return blnSuccess;
        }

        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class FileGuardianConfiguration
    {
        public static FileGuardianConfiguration Default => new FileGuardianConfiguration();

        public Dictionary<DirectoryInfo, List<DirectoryInfo>> SourceToTargetDirectories { get; set; } = new Dictionary<DirectoryInfo, List<DirectoryInfo>>();

        // TODO: Implement
        public bool IsValid => false;
    }
}
