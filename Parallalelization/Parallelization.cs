namespace Parallalelization
{
    public static class Parallelization
    {
        /// <summary>
        /// Asynchronously parallelize callback operations divided into concurrently operations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="parallelSize">How divided the collection tasks will be</param>
        /// <param name="callback">The operation itself</param>
        /// <param name="descriptiveOperations">True to print the step-by-step</param>
        /// <returns></returns>
        public static async Task ParallelizeAsync<T>(this IEnumerable<T> collection, int parallelSize, Func<T, Task> callback, bool descriptiveOperations = false)
        {
            Validate(collection, parallelSize, callback);

            var loopTaskList = new List<Task>();

            for (int i = 0; i < parallelSize; i++)
            {
                if (descriptiveOperations)
                    Console.WriteLine($"INITIALIZING PARALLELIZATION Nº {i}");

                var currentLoop = collection
                    .Skip(i * collection.Count() / parallelSize)
                    .Take(collection.Count() / parallelSize)
                    .Select(task => new { Task = task, Pod = i })
                    .ToList();

                var loopTask = Task.Run(async () =>
                {
                    foreach (var item in currentLoop.Select((model, index) => (model, index)))
                    {
                        if (descriptiveOperations)
                            Console.WriteLine($"POD {item.model.Pod + 1} - PROCESSING TASK INDEX {item.index} OF {currentLoop.Count - 1}");

                        await callback.Invoke(item.model.Task);

                        if (descriptiveOperations)
                            Console.WriteLine($"POD {item.model.Pod + 1} - TASK INDEX {item.index} PROCESSED. {currentLoop.Count - 1 - item.index} TASKS REMAINING");
                    }
                });

                loopTaskList.Add(loopTask);
            }

            await Task.WhenAll(loopTaskList);            
        }

        /// <summary>
        /// Parallelize callback operations divided into concurrently operations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="parallelSize">How divided the collection tasks will be</param>
        /// <param name="callback">The operation itself</param>
        /// <param name="descriptiveOperations">True to print the step-by-step</param>
        /// <returns></returns>
        public static void Parallelize<T>(this IEnumerable<T> collection, int parallelSize, Func<T, Task> callback, bool descriptiveOperations = false)
        {
            Validate(collection, parallelSize, callback);

            var loopTaskList = new List<Task>();

            for (int i = 0; i < parallelSize; i++)
            {
                if (descriptiveOperations)
                    Console.WriteLine($"INITIALIZING PARALLELIZATION Nº {i}");

                var currentLoop = collection
                    .Skip(i * collection.Count() / parallelSize)
                    .Take(collection.Count() / parallelSize)
                    .Select(task => new { Task = task, Pod = i })
                    .ToList();

                var loopTask = Task.Run(async () =>
                {
                    foreach (var item in currentLoop.Select((model, index) => (model, index)))
                    {
                        if (descriptiveOperations)
                            Console.WriteLine($"POD {item.model.Pod + 1} - PROCESSING TASK INDEX {item.index} OF {currentLoop.Count - 1}");

                        await callback.Invoke(item.model.Task);

                        if (descriptiveOperations)
                            Console.WriteLine($"POD {item.model.Pod + 1} - TASK INDEX {item.index} PROCESSED. {currentLoop.Count - 1 - item.index} TASKS REMAINING");
                    }
                });

                loopTaskList.Add(loopTask);
            }

            Task.WaitAll(loopTaskList.ToArray());
        }

        private static void Validate<T>(IEnumerable<T> collection, int parallelSize, Func<T, Task> callback)
        {
            List<string> errors = new();

            if (collection == null)
                errors.Add("Collection can't be null");

            if (parallelSize < 2)
                errors.Add("ParallelSize can't be less than 2");

            if (parallelSize > collection.Count())
                errors.Add("ParallelSize can't be bigger than collection size");

            if (callback is null)
                errors.Add("Callback can't be null");

            if (errors.Any())
                throw new ApplicationException(string.Join("\n", errors));
        }
    }
}