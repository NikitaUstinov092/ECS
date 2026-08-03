using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace SampleGame
{
    public static unsafe class UnsafeUseCase
    {
        public static T* AllocPointer<T>(T value, Allocator allocator) where T : unmanaged
        {
            T* ptr = (T*) UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), allocator);
            *ptr = value;
            return ptr;
        }

        public static void FreePointer<T>(T* ptr, Allocator allocator) where T : unmanaged
        {
            if (ptr != null)
                UnsafeUtility.Free(ptr, allocator);
        }
    }
}