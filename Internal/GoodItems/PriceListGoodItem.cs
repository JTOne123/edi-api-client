﻿using KonturEdi.Api.Types.Organization;

namespace KonturEdi.Api.Types.Internal.GoodItems
{
    public class PriceListGoodItem : GoodItemBase
    {
        public PricatGoodItemStatus? PricatGoodItemStatus { get; set; }

        public bool IsVariableQuantityProduct { get; set; }

        public string Name { get; set; }
        public string BuyerName { get; set; }

        public string Color { get; set; }
        public string Size { get; set; }
        public string BrandName { get; set; }

        public string CalculationGroup { get; set; }

        public Quantity Height { get; set; }
        public Quantity Width { get; set; }
        public Quantity Depth { get; set; }
        public Quantity NetContent { get; set; }
        public Quantity GrossWeight { get; set; }
        public Quantity PriceQuantity { get; set; }
        public Quantity OnePlaceQuantity { get; set; }
        public Quantity IncrementalOrderQuantity { get; set; }
        public Quantity QuantityInPackage { get; set; }

        public string[] CountriesOfOriginCode { get; set; }
        public string[] CustomDeclarationNumbers { get; set; }

        public decimal? ExciseTax { get; set; }
        public string VATRate { get; set; }

        public Price Price { get; set; }
        public Price OldPriceWithVAT { get; set; }
        public Price PriceWithVAT { get; set; }

        public Price CostOfInstallation { get; set; }

        public string Package { get; set; }
        public string PackageType { get; set; }

        public PartyInfo Manufacturer { get; set; }

        public string Comment { get; set; }

        public bool DevelopmentMode { get; set; }
    }

    public class Price
    {
        public decimal? Value { get; set; }
        public string MeasurementUnitCode { get; set; }
    }

    public enum PricatGoodItemStatus
    {
        Empty,
        Added,
        NotFound,
        NoAction,
        Changed,
        Deleted,
    }
}