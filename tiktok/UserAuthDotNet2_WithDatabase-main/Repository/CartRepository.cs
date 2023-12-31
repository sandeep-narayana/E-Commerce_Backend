using Dapper;

namespace UserAuthDotBet2_WithDatabase.Repositories;

public interface ICartRepository
{
    public Task<List<Cart>> getCartById(int UserId);
    public Task<string> AddToCart(Product product, int userId);

    public Task<bool> deleteProductById(int ProductId, int userId);

    public Task<bool> changeQuantity(int cartId, int newQuantity, int userId);
}

public class CartRepository : BaseRepository, ICartRepository
{
    public CartRepository(IConfiguration config) : base(config)
    {
    }


    public async Task<List<Cart>> getCartById(int UserId)
    {
        var query = "SELECT * FROM cart where user_id = @UserId";
        using var con = NewConnection;

        var res = await con.QueryAsync<Cart>(query, new { UserId = UserId });
        return res.AsList();
    }

    public async Task<string> AddToCart(Product product, int userId)
    {
        var query = "INSERT INTO cart (name, description, image, price, user_id, quantity, product_id) VALUES (@Name, @Description, @Image, @Price, @UserId, @Quantity, @ProductId)";

        using var con = NewConnection;

        var res = await con.ExecuteAsync(query, new
        {
            UserId = userId,
            Name = product.Name,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price,
            Quantity = 1, // You can set the quantity to 1 when adding a single product to the cart
            ProductId = product.Id
        });

        if (res > 0)
        {
            return "Product added to cart successfully";
        }
        else
        {
            return "Failed to add the product to the cart";
        }
    }

    public async Task<bool> deleteProductById(int ProductId, int UserId)
    {

        var query = "DELETE FROM cart WHERE product_id = @ProductId AND user_id = @UserId";
        var con = NewConnection;
        return await con.ExecuteAsync(query, new
        {
            ProductId = ProductId,
            UserId = UserId
        }) > 0;
    }

    public async Task<bool> changeQuantity(int cartId, int newQuantity, int userId)
    {
        var query = "UPDATE cart SET quantity = @NewQuantity WHERE id = @Id AND user_id = @UserId";
        var con = NewConnection;

        return await con.ExecuteAsync(query, new
        {
            NewQuantity = newQuantity,
            Id = cartId,
            UserId = userId

        }) > 0;
    }
}