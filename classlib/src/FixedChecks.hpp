template<typename T>
std::gc_ptr<Qt::QSharp::FixedArray1D<T>>& $check(std::gc_ptr<Qt::QSharp::FixedArray1D<T>> &array) {
  return array;
}

template<typename T>
std::gc_ptr<Qt::QSharp::FixedArray1D<T>>* $check(std::gc_ptr<Qt::QSharp::FixedArray1D<T>>* array) {
  return array;
}

template<typename T>
std::gc_ptr<Qt::QSharp::FixedArray1D<T>>& $check(std::gc_ptr<Qt::QSharp::FixedArray1D<T>> &array, int offset, int length)
{if (offset + length > array->Length) $abe(); return array;}

template<typename T>
std::gc_ptr<Qt::QSharp::FixedArray2D<T>>& $check(std::gc_ptr<Qt::QSharp::FixedArray2D<T>> &array) {
  return array;
}

template<typename T>
std::gc_ptr<Qt::QSharp::FixedArray2D<T>>* $check(std::gc_ptr<Qt::QSharp::FixedArray2D<T>>* array) {
  return array;
}

template<typename T>
std::gc_ptr<Qt::QSharp::FixedArray2D<T>>& $check(std::gc_ptr<Qt::QSharp::FixedArray2D<T>> &array, int offset, int length)
{if (offset + length > array->Length) $abe(); return array;}

template<typename T>
std::gc_ptr<Qt::QSharp::FixedArray3D<T>>& $check(std::gc_ptr<Qt::QSharp::FixedArray3D<T>> &array) {
  return array;
}

template<typename T>
std::gc_ptr<Qt::QSharp::FixedArray3D<T>>* $check(std::gc_ptr<Qt::QSharp::FixedArray3D<T>>* array) {
  return array;
}

template<typename T>
std::gc_ptr<Qt::QSharp::FixedArray3D<T>>& $check(std::gc_ptr<Qt::QSharp::FixedArray3D<T>> &array, int offset, int length)
{if (offset + length > array->Length) $abe(); return array;}

