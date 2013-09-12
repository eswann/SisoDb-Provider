using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SisoDb
{
    /// <summary>
    /// Lets you perform Inserts, Updates, Deletes and Queries against the <see cref="ISisoDatabase"/>.
    /// This API should only be used if you perform a single call against the Db. If you make more than
    /// one subsequent call right after each other, YOU SHOULD USE <see cref="ISisoDatabase.BeginSession"/>
    /// INSTEAD.
    /// </summary>
	public interface ISingleOperationSession 
	{
        /// <summary>
        /// Lets you perform a Query defining things like
        /// <see cref="ISisoQueryable{T}.Take"/>
        /// <see cref="ISisoQueryable{T}.Where"/>
        /// <see cref="ISisoQueryable{T}.OrderBy"/>
        /// <see cref="ISisoQueryable{T}.Page"/>
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns></returns>
        /// <remarks>Does not consume any <see cref="ICacheProvider"/>.</remarks>
        ISisoQueryable<T> Query<T>() where T : class;

        /// <summary>
        /// Returns the StructureIds for <typeparamref name="T"/> as <typeparamref name="TId"/> withoout
        /// having to deserialize the structure. Hence this is more effective then getting the structure
        /// and then extracting the id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TId[] GetIds<T, TId>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Returns the StructureIds for <typeparamref name="T"/> as <typeparamref name="TId"/> withoout
        /// having to deserialize the structure. Hence this is more effective then getting the structure
        /// and then extracting the id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TId[]> GetIdsAsync<T, TId>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Returns the StructureIds for <typeparamref name="T"/> as <see cref="object"/> withoout
        /// having to deserialize the structure. Hence this is more effective then getting the structure
        /// and then extracting the id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        object[] GetIds<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Returns the StructureIds for <typeparamref name="T"/> as <see cref="object"/> withoout
        /// having to deserialize the structure. Hence this is more effective then getting the structure
        /// and then extracting the id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<object[]> GetIdsAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Used to find one single instance or NULL or <typeparamref name="T"/>. The benefits over <see cref="Query{T}"/>,
        /// is that <see cref="GetByQuery{T}"/> consumes any present <see cref="ICacheProvider"/>, which <see cref="Query{T}"/> does not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        T GetByQuery<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Used to find one single instance or NULL or <typeparamref name="T"/>. The benefits over <see cref="Query{T}"/>,
        /// is that <see cref="GetByQuery{T}"/> consumes any present <see cref="ICacheProvider"/>, which <see cref="Query{T}"/> does not.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<T> GetByQueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Returns one single structure identified by an id.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
        /// <returns>Structure for (<typeparamref name="T"/>) or NULL.</returns>
	    T GetById<T>(object id) where T : class;


        /// <summary>
        /// Returns one single structure identified by an id.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
        /// <returns>Structure for (<typeparamref name="T"/>) or NULL.</returns>
        Task<T> GetByIdAsync<T>(object id) where T : class;

        /// <summary>
        /// Returns one single structure identified by an id.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="id"></param>
        /// <returns>Structure for (<paramref name="structureType"/>) matching <paramref name="id"/> or NULL.</returns>
        object GetById(Type structureType, object id);

        /// <summary>
        /// Returns one single structure identified by an id.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="id"></param>
        /// <returns>Structure for (<paramref name="structureType"/>) matching <paramref name="id"/> or NULL.</returns>
        Task<object> GetByIdAsync(Type structureType, object id);

        /// <summary>
        /// Returns one single structure identified
        /// by an id. 
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="id"></param>
        /// <returns>Structure (<typeparamref name="TOut"/>) or NULL.</returns>
	    TOut GetByIdAs<TContract, TOut>(object id) where TContract : class where TOut : class;

        /// <summary>
        /// Returns one single structure identified
        /// by an id. 
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="id"></param>
        /// <returns>Structure (<typeparamref name="TOut"/>) or NULL.</returns>
        Task<TOut> GetByIdAsAsync<TContract, TOut>(object id)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Returns one single structure identified
        /// by an id, as Json. This is the most
        /// effective return type, since the Json
        /// is stored in the database, no deserialization
        /// will take place.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
        /// <returns>Json representation of (<typeparamref name="T"/>) or Null</returns>
        string GetByIdAsJson<T>(object id) where T : class;

        /// <summary>
        /// Returns one single structure identified
        /// by an id, as Json. This is the most
        /// effective return type, since the Json
        /// is stored in the database, no deserialization
        /// will take place.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
        /// <returns>Json representation of (<typeparamref name="T"/>) or Null</returns>
        Task<string> GetByIdAsJsonAsync<T>(object id) where T : class;

        /// <summary>
        /// Returns one single structure identified
        /// by an id, as Json. This is the most
        /// effective return type, since the Json
        /// is stored in the database, no deserialization
        /// will take place.  
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="id"></param>
        /// <returns>Json representation of (<paramref name="structureType"/>) or Null</returns>
        string GetByIdAsJson(Type structureType, object id);

        /// <summary>
        /// Returns one single structure identified
        /// by an id, as Json. This is the most
        /// effective return type, since the Json
        /// is stored in the database, no deserialization
        /// will take place.  
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="id"></param>
        /// <returns>Json representation of (<paramref name="structureType"/>) or Null</returns>
        Task<string> GetByIdAsJsonAsync(Type structureType, object id);

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable of <typeparamref name="T"/>.</returns>
	    T[] GetByIds<T>(params object[] ids) where T : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable of <typeparamref name="T"/>.</returns>
        Task<T[]> GetByIdsAsync<T>(params object[] ids) where T : class;

        /// <summary>
        /// Returns all structures for the defined structure type <paramref name="structureType"/>
        /// that matches passed ids.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        object[] GetByIds(Type structureType, params object[] ids);

        /// <summary>
        /// Returns all structures for the defined structure type <paramref name="structureType"/>
        /// that matches passed ids.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<object[]> GetByIdsAsync(Type structureType, params object[] ids);

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="TContract"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable of <typeparamref name="TOut"/>.</returns>
	    TOut[] GetByIdsAs<TContract, TOut>(params object[] ids) where TContract : class where TOut : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="TContract"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable of <typeparamref name="TOut"/>.</returns>
        Task<TOut[]> GetByIdsAsAsync<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Returns Json representation for all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable Json representation of <typeparamref name="T"/>.</returns>
        string[] GetByIdsAsJson<T>(params object[] ids) where T : class;

        /// <summary>
        /// Returns Json representation for all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable Json representation of <typeparamref name="T"/>.</returns>
        Task<string[]> GetByIdsAsJsonAsync<T>(params object[] ids) where T : class;

        /// <summary>
        /// Returns Json representation for all structures for the defined structure <paramref name="structureType"/>
        /// matching passed ids.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable Json representation of <paramref name="structureType"/>.</returns>
        string[] GetByIdsAsJson(Type structureType, params object[] ids);

        /// <summary>
        /// Returns Json representation for all structures for the defined structure <paramref name="structureType"/>
        /// matching passed ids.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable Json representation of <paramref name="structureType"/>.</returns>
        Task<string[]> GetByIdsAsJsonAsync(Type structureType, params object[] ids);

        /// <summary>
        /// Inserts a single structure using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="item"></param>
	    void Insert<T>(T item) where T : class;

        /// <summary>
        /// Inserts a single structure using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="item"></param>
        Task InsertAsync<T>(T item) where T : class;


        /// <summary>
        /// Inserts a single structure using the <paramref name="structureType"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="item"></param>
        void Insert(Type structureType, object item);

        /// <summary>
        /// Inserts a single structure using the <paramref name="structureType"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="item"></param>
        Task InsertAsync(Type structureType, object item);

        /// <summary>
        /// Inserts a single structure using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted. As item, you can pass
        /// any type that has partial or full match of the contract, without extending it.
        /// E.g An anonymous type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        void InsertAs<T>(object item) where T : class;

        /// <summary>
        /// Inserts a single structure using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted. As item, you can pass
        /// any type that has partial or full match of the contract, without extending it.
        /// E.g An anonymous type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        Task InsertAsAsync<T>(object item) where T : class;

        /// <summary>
        /// Inserts a single structure using the <paramref name="structureType"/> as
        /// the contract for the structure being inserted. As item, you can pass
        /// any type that has partial or full match of the contract, without extending it.
        /// E.g An anonymous type.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="item"></param>
        void InsertAs(Type structureType, object item);

        /// <summary>
        /// Inserts a single structure using the <paramref name="structureType"/> as
        /// the contract for the structure being inserted. As item, you can pass
        /// any type that has partial or full match of the contract, without extending it.
        /// E.g An anonymous type.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="item"></param>
        Task InsertAsAsync(Type structureType, object item);

        /// <summary>
        /// Inserts Json strcutures using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structure will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns>The Json for the item being inserted, but after insert so that the Id is included.</returns>
        string InsertJson<T>(string json) where T : class;

        /// <summary>
        /// Inserts Json strcutures using the <typeparamref name="T"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structure will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns>The Json for the item being inserted, but after insert so that the Id is included.</returns>
        Task<string> InsertJsonAsync<T>(string json) where T : class;

        /// <summary>
        /// Inserts Json strcutures using the <paramref name="structureType"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structure will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <param name="structureType"></param>
        /// <param name="json"></param>
        /// <returns>The Json for the item being inserted, but after insert so that the Id is included.</returns>
        string InsertJson(Type structureType, string json);

        /// <summary>
        /// Inserts Json strcutures using the <paramref name="structureType"/> as
        /// the contract for the structure being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structure will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <param name="structureType"></param>
        /// <param name="json"></param>
        /// <returns>The Json for the item being inserted, but after insert so that the Id is included.</returns>
        Task<string> InsertJsonAsync(Type structureType, string json);

        /// <summary>
        /// Inserts multiple structures using the <typeparamref name="T"/> as
        /// the contract for the structures being inserted. 
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="items"></param>
	    void InsertMany<T>(IEnumerable<T> items) where T : class;

        /// <summary>
        /// Inserts multiple structures using the <typeparamref name="T"/> as
        /// the contract for the structures being inserted. 
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="items"></param>
        Task InsertManyAsync<T>(IEnumerable<T> items) where T : class;

        /// <summary>
        /// Inserts multiple structures using the <paramref name="structureType"/> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="items"></param>
        void InsertMany(Type structureType, IEnumerable<object> items);

        /// <summary>
        /// Inserts multiple structures using the <paramref name="structureType"/> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <param name="structureType"></param>
        /// <param name="items"></param>
        Task InsertManyAsync(Type structureType, IEnumerable<object> items);

        /// <summary>
        /// Inserts multiple Json strcutures using the <typeparamref name="T"/> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structures will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        void InsertManyJson<T>(IEnumerable<string> json) where T : class;

        /// <summary>
        /// Inserts multiple Json strcutures using the <typeparamref name="T"/> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structures will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        Task InsertManyJsonAsync<T>(IEnumerable<string> json) where T : class;

        /// <summary>
        /// Inserts multiple Json strcutures using the <paramref name="structureType"/> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structures will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <param name="structureType"></param>
        /// <param name="json"></param>
        void InsertManyJson(Type structureType, IEnumerable<string> json);

        /// <summary>
        /// Inserts multiple Json strcutures using the <paramref name="structureType"/> as
        /// the contract for the structures being inserted.
        /// </summary>
        /// <remarks>Deserialization of the Json structures will take place, 
        /// so If you do have the instace pass that instead using other overload!</remarks>
        /// <param name="structureType"></param>
        /// <param name="json"></param>
        Task InsertManyJsonAsync(Type structureType, IEnumerable<string> json);

        /// <summary>
        /// Updates the sent structure. If it
        /// does not exist, an <see cref="SisoDbException"/> will be
        /// thrown.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="item"></param>
	    void Update<T>(T item) where T : class;

        /// <summary>
        /// Updates the sent structure. If it
        /// does not exist, an <see cref="SisoDbException"/> will be
        /// thrown.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="item"></param>
        Task UpdateAsync<T>(T item) where T : class;

        /// <summary>
        /// Updates the sent structure. If it
        /// does not exist, an <see cref="SisoDbException"/> will be
        /// thrown.
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="item"></param>
        void Update(Type structureType, object item);

        /// <summary>
        /// Updates the sent structure. If it
        /// does not exist, an <see cref="SisoDbException"/> will be
        /// thrown.
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="item"></param>
        Task UpdateAsync(Type structureType, object item);

        /// <summary>
        /// Uses sent id to locate a structure and then calls sent <paramref name="modifier"/>
        /// to apply the changes. Will also place an rowlock, which makes it highly
        /// useful in a concurrent environment like in an event denormalizer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <param name="proceed">True to continue with update;False to abort</param>
        void Update<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class;

        /// <summary>
        /// Uses sent id to locate a structure and then calls sent <paramref name="modifier"/>
        /// to apply the changes. Will also place an rowlock, which makes it highly
        /// useful in a concurrent environment like in an event denormalizer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="modifier"></param>
        /// <param name="proceed">True to continue with update;False to abort</param>
        Task UpdateAsync<T>(object id, Action<T> modifier, Func<T, bool> proceed = null) where T : class;

        /// <summary>
        /// Traverses every structure in the set and lets you apply changes to each yielded structure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="modifier"></param>
        /// <remarks>Does not support Concurrency tokens</remarks>
        void UpdateMany<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class;

        /// <summary>
        /// Traverses every structure in the set and lets you apply changes to each yielded structure.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="modifier"></param>
        /// <remarks>Does not support Concurrency tokens</remarks>
        Task UpdateManyAsync<T>(Expression<Func<T, bool>> predicate, Action<T> modifier) where T : class;

        /// <summary>
        /// Clears all stored structures of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void Clear<T>() where T : class;

        /// <summary>
        /// Clears all stored structures of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        Task ClearAsync<T>() where T : class;

        /// <summary>
        /// Clears all stored structures of type specified by <paramref name="structureType"/>.
        /// </summary>
        /// <param name="structureType"></param>
        void Clear(Type structureType);

        /// <summary>
        /// Clears all stored structures of type specified by <paramref name="structureType"/>.
        /// </summary>
        /// <param name="structureType"></param>
        Task ClearAsync(Type structureType);

        /// <summary>
        /// Deletes all items except items having an id present in <paramref name="ids"/>.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids for the structures to keep.</param>
        void DeleteAllExceptIds<T>(params object[] ids) where T : class;

        /// <summary>
        /// Deletes all items except items having an id present in <paramref name="ids"/>.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids for the structures to keep.</param>
        Task DeleteAllExceptIdsAsync<T>(params object[] ids) where T : class;

        /// <summary>
        /// Deletes all items except items having an id present in <paramref name="ids"/>.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids"></param>
        void DeleteAllExceptIds(Type structureType, params object[] ids);

        /// <summary>
        /// Deletes all items except items having an id present in <paramref name="ids"/>.
        /// </summary>
        /// <param name="structureType">Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids"></param>
        Task DeleteAllExceptIdsAsync(Type structureType, params object[] ids);

        /// <summary>
        /// Deletes structure by id.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
	    void DeleteById<T>(object id) where T : class;

        /// <summary>
        /// Deletes structure by id.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
        Task DeleteByIdAsync<T>(object id) where T : class;

        /// <summary>
        /// Deletes structure by id.
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="id"></param>
        void DeleteById(Type structureType, object id);

        /// <summary>
        /// Deletes structure by id.
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="id"></param>
        Task DeleteByIdAsync(Type structureType, object id);

        /// <summary>
        /// Deletes all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to delete.</param>
	    void DeleteByIds<T>(params object[] ids) where T : class;

        /// <summary>
        /// Deletes all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to delete.</param>
        Task DeleteByIdsAsync<T>(params object[] ids) where T : class;

        /// <summary>
        /// Deletes all structures for the defined structure <paramref name="structureType"/>
        /// matching passed ids.
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids">Ids used for matching the structures to delete.</param>
        void DeleteByIds(Type structureType, params object[] ids);

        /// <summary>
        /// Deletes all structures for the defined structure <paramref name="structureType"/>
        /// matching passed ids.
        /// </summary>
        /// <param name="structureType">
        /// Structure type, used as a contract defining the scheme.</param>
        /// <param name="ids">Ids used for matching the structures to delete.</param>
        Task DeleteByIdsAsync(Type structureType, params object[] ids);

        /// <summary>
        /// Deletes one or more structures matchings the sent
        /// predicate.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="predicate"></param>
        void DeleteByQuery<T>(Expression<Func<T, bool>> predicate) where T : class;

        /// <summary>
        /// Deletes one or more structures matchings the sent
        /// predicate.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="predicate"></param>
        Task DeleteByQueryAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
	}
}